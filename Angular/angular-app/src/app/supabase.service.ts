import { Injectable } from '@angular/core';
import { createClient } from '@supabase/supabase-js';
import { environment } from '../environments/environment';

import type { UserProfile } from './profile/profile.types';

@Injectable({ providedIn: 'root' })
export class SupabaseService {
  client = createClient(
    environment.supabaseUrl,
    environment.supabaseKey,
    {
      auth: {
        persistSession: true,
        autoRefreshToken: true,
      },
    },
  );

  signUp(email: string, password: string) {
    return this.client.auth.signUp({ email, password });
  }

  signIn(email: string, password: string) {
    return this.client.auth.signInWithPassword({ email, password });
  }

  signOut() {
    return this.client.auth.signOut();
  }

  getUser() {
    return this.client.auth.getUser();
  }

  getSession() {
    return this.client.auth.getSession();
  }

  /** Loads `public.profiles` for the given user id. Returns `null` if no row exists. */
  getProfile(userId: string) {
    return this.client
      .from('profiles')
      .select('id, full_name, avatar_url, bio, contact_number, address, location, membership, role, updated_at')
      .eq('id', userId)
      .maybeSingle<UserProfile>();
  }

  /** Creates or updates the current user's profile row. */
  upsertProfile(
    userId: string,
    payload: {
      full_name: string | null;
      bio: string | null;
      avatar_url: string | null;
      contact_number: string | null;
      address: string | null;
      location: string | null;
      membership: 'Premium' | 'Social';
    },
  ) {
    return this.client
      .from('profiles')
      .upsert(
        {
          id: userId,
          full_name: payload.full_name,
          bio: payload.bio,
          avatar_url: payload.avatar_url,
          contact_number: payload.contact_number,
          address: payload.address,
          location: payload.location,
          membership: payload.membership,
          updated_at: new Date().toISOString(),
        },
        { onConflict: 'id' },
      )
      .select('id, full_name, avatar_url, bio, contact_number, address, location, membership, role, updated_at')
      .single<UserProfile>();
  }
}