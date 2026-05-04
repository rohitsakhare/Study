/** Row shape for `public.profiles` (see `supabase/profiles.sql`). */
export type MembershipTier = 'Premium' | 'Social';

export type ProfileRole = 'user' | 'admin';

export interface UserProfile {
  id: string;
  full_name: string | null;
  avatar_url: string | null;
  bio: string | null;
  contact_number: string | null;
  address: string | null;
  location: string | null;
  membership: MembershipTier | null;
  role: ProfileRole | null;
  updated_at: string | null;
}
