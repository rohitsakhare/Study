import 'dotenv/config';
import {
  AngularNodeAppEngine,
  createNodeRequestHandler,
  isMainModule,
  writeResponseToNodeResponse,
} from '@angular/ssr/node';
import { createClient } from '@supabase/supabase-js';
import express, { type Request, type Response } from 'express';
import { join } from 'node:path';

const browserDistFolder = join(import.meta.dirname, '../browser');

const supabaseUrl = process.env['SUPABASE_URL'] ?? '';
const supabaseAnonKey = process.env['SUPABASE_ANON_KEY'] ?? '';
const supabaseServiceKey = process.env['SUPABASE_SERVICE_ROLE_KEY'] ?? '';

const app = express();
const angularApp = new AngularNodeAppEngine();

app.use(express.json({ limit: '32kb' }));

/**
 * Helper: require a valid admin session and return a service-role Supabase client.
 */
async function requireAdminClient(req: Request, res: Response) {
  if (!supabaseUrl || !supabaseAnonKey || !supabaseServiceKey) {
    res.status(503).json({
      error:
        'Server missing Supabase env. Create a .env file with SUPABASE_URL, SUPABASE_ANON_KEY, and SUPABASE_SERVICE_ROLE_KEY and restart the SSR server.',
    });
    return null;
  }

  const authHeader = req.headers['authorization'];
  if (typeof authHeader !== 'string' || !authHeader.startsWith('Bearer ')) {
    res.status(401).json({ error: 'Missing authorization' });
    return null;
  }

  const token = authHeader.slice(7).trim();
  if (!token) {
    res.status(401).json({ error: 'Missing authorization token' });
    return null;
  }

  const userClient = createClient(supabaseUrl, supabaseAnonKey, {
    global: { headers: { Authorization: `Bearer ${token}` } },
  });

  const {
    data: { user },
    error: userErr,
  } = await userClient.auth.getUser(token);
  if (userErr || !user) {
    res.status(401).json({ error: 'Invalid session' });
    return null;
  }

  const { data: profile, error: profErr } = await userClient
    .from('profiles')
    .select('role')
    .eq('id', user.id)
    .maybeSingle();

  if (profErr) {
    res.status(500).json({ error: profErr.message });
    return null;
  }
  if (profile?.role !== 'admin') {
    res.status(403).json({ error: 'Admin access required' });
    return null;
  }

  return createClient(supabaseUrl, supabaseServiceKey, {
    auth: {
      persistSession: false,
      autoRefreshToken: false,
      detectSessionInUrl: false,
    },
  });
}

app.get('/api/admin/users', async (req, res) => {
  const adminClient = await requireAdminClient(req, res);
  if (!adminClient) {
    return;
  }

  const { data: listData, error: listErr } = await adminClient.auth.admin.listUsers({
    perPage: 100,
  });
  if (listErr) {
    res.status(500).json({ error: listErr.message });
    return;
  }

  const users = listData?.users ?? [];
  const ids = users.map((user) => user.id);

  const { data: profileRows, error: profileErr } = await adminClient
    .from('profiles')
    .select('id, full_name, avatar_url, bio, contact_number, address, location, membership, role')
    .in('id', ids);

  if (profileErr) {
    res.status(500).json({ error: profileErr.message });
    return;
  }

  const profilesById = new Map(profileRows?.map((profile) => [profile.id, profile]));

  const response = users.map((user) => {
    const profile = profilesById.get(user.id);
    return {
      id: user.id,
      email: user.email ?? null,
      full_name: profile?.full_name ?? null,
      bio: profile?.bio ?? null,
      avatar_url: profile?.avatar_url ?? null,
      contact_number: profile?.contact_number ?? null,
      address: profile?.address ?? null,
      location: profile?.location ?? null,
      membership: profile?.membership === 'Premium' ? 'Premium' : 'Social',
      role: profile?.role === 'admin' ? 'admin' : 'user',
    };
  });

  res.json(response);
});

app.post('/api/admin/users', async (req, res) => {
  const adminClient = await requireAdminClient(req, res);
  if (!adminClient) {
    return;
  }

  const email = typeof req.body?.email === 'string' ? req.body.email.trim() : '';
  const password = typeof req.body?.password === 'string' ? req.body.password : '';

  if (!email || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    res.status(400).json({ error: 'Valid email required' });
    return;
  }
  if (password.length < 6) {
    res.status(400).json({ error: 'Password must be at least 6 characters' });
    return;
  }

  const { data: created, error: createErr } = await adminClient.auth.admin.createUser({
    email,
    password,
    email_confirm: true,
  });

  if (createErr) {
    res.status(400).json({ error: createErr.message });
    return;
  }

  const newUser = created.user;
  if (!newUser) {
    res.status(500).json({ error: 'User created but no user returned' });
    return;
  }

  const { error: profileInsertErr } = await adminClient.from('profiles').upsert(
    {
      id: newUser.id,
      role: 'user',
      membership: 'Social',
      full_name: null,
      bio: null,
      contact_number: null,
      address: null,
      location: null,
      avatar_url: null,
    },
    { onConflict: 'id' },
  );

  if (profileInsertErr) {
    res.status(500).json({
      error: `Auth user created but profile upsert failed: ${profileInsertErr.message}`,
    });
    return;
  }

  res.status(201).json({ message: 'User created', id: newUser.id });
});

app.patch('/api/admin/users/:id', async (req, res) => {
  const adminClient = await requireAdminClient(req, res);
  if (!adminClient) {
    return;
  }

  const userId = typeof req.params?.id === 'string' ? req.params.id : '';
  if (!userId) {
    res.status(400).json({ error: 'User id is required' });
    return;
  }

  const email = typeof req.body?.email === 'string' ? req.body.email.trim() : undefined;
  const full_name = typeof req.body?.full_name === 'string' ? req.body.full_name.trim() : undefined;
  const bio = typeof req.body?.bio === 'string' ? req.body.bio.trim() : undefined;
  const contact_number = typeof req.body?.contact_number === 'string' ? req.body.contact_number.trim() : undefined;
  const address = typeof req.body?.address === 'string' ? req.body.address.trim() : undefined;
  const location = typeof req.body?.location === 'string' ? req.body.location.trim() : undefined;
  const avatar_url = typeof req.body?.avatar_url === 'string' ? req.body.avatar_url.trim() : undefined;
  const membership = req.body?.membership;
  const role = req.body?.role;

  if (email !== undefined && email !== null && email !== '' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    res.status(400).json({ error: 'Valid email required' });
    return;
  }
  if (membership !== undefined && membership !== 'Premium' && membership !== 'Social') {
    res.status(400).json({ error: 'Membership must be Premium or Social' });
    return;
  }
  if (role !== undefined && role !== 'admin' && role !== 'user') {
    res.status(400).json({ error: 'Role must be admin or user' });
    return;
  }

  if (email) {
    const { error: emailErr } = await adminClient.auth.admin.updateUserById(userId, {
      email,
      email_confirm: true,
    });
    if (emailErr) {
      res.status(400).json({ error: emailErr.message });
      return;
    }
  }

  const profileUpdate: {
    id: string;
    full_name?: string | null;
    bio?: string | null;
    contact_number?: string | null;
    address?: string | null;
    location?: string | null;
    avatar_url?: string | null;
    membership?: string;
    role?: string;
  } = { id: userId };

  if (full_name !== undefined) profileUpdate.full_name = full_name === '' ? null : full_name;
  if (bio !== undefined) profileUpdate.bio = bio === '' ? null : bio;
  if (contact_number !== undefined) profileUpdate.contact_number = contact_number === '' ? null : contact_number;
  if (address !== undefined) profileUpdate.address = address === '' ? null : address;
  if (location !== undefined) profileUpdate.location = location === '' ? null : location;
  if (avatar_url !== undefined) profileUpdate.avatar_url = avatar_url === '' ? null : avatar_url;
  if (membership !== undefined) profileUpdate.membership = membership;
  if (role !== undefined) profileUpdate.role = role;

  let updatedProfile = null;
  if (Object.keys(profileUpdate).length > 1) {
    const { data: profileData, error: profileErr } = await adminClient
      .from('profiles')
      .upsert(profileUpdate, { onConflict: 'id' })
    .select('id, full_name, avatar_url, bio, contact_number, address, location, membership, role')
    if (profileErr) {
      res.status(500).json({ error: profileErr.message });
      return;
    }
    updatedProfile = profileData;
  }

  const {
    data: { user: userData },
    error: userErr,
  } = await adminClient.auth.admin.getUserById(userId);
  if (userErr || !userData) {
    res.status(404).json({ error: 'User not found' });
    return;
  }

  const { data: profileRow, error: profileFetchErr } = await adminClient
    .from('profiles')
    .select('full_name, avatar_url, bio, contact_number, address, location, membership, role')
    .eq('id', userId)
    .maybeSingle();

  if (profileFetchErr) {
    res.status(500).json({ error: profileFetchErr.message });
    return;
  }

  res.json({
    id: userData.id,
    email: userData.email ?? null,
    full_name: profileRow?.full_name ?? null,
    bio: profileRow?.bio ?? null,
    contact_number: profileRow?.contact_number ?? null,
    address: profileRow?.address ?? null,
    location: profileRow?.location ?? null,
    avatar_url: profileRow?.avatar_url ?? null,
    membership: profileRow?.membership === 'Premium' ? 'Premium' : 'Social',
    role: profileRow?.role === 'admin' ? 'admin' : 'user',
  });
});

/**
 * Serve static files from /browser
 */
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  }),
);

/**
 * Handle all other requests by rendering the Angular application.
 */
app.use((req, res, next) => {
  angularApp
    .handle(req)
    .then((response) =>
      response ? writeResponseToNodeResponse(response, res) : next(),
    )
    .catch(next);
});

/**
 * Start the server if this module is the main entry point, or it is ran via PM2.
 */
if (isMainModule(import.meta.url) || process.env['pm_id']) {
  const port = process.env['PORT'] || 4000;
  app.listen(port, (error) => {
    if (error) {
      throw error;
    }

    console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

/**
 * Request handler used by the Angular CLI (for dev-server and during build) or Firebase Cloud Functions.
 */
export const reqHandler = createNodeRequestHandler(app);
