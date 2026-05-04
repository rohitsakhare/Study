-- Run in Supabase: SQL Editor → New query → paste → Run.
-- Creates a profiles table keyed to auth.users and basic RLS so the anon key can read/update own row.

create table if not exists public.profiles (
  id uuid primary key references auth.users (id) on delete cascade,
  full_name text,
  avatar_url text,
  bio text,
  contact_number text,
  address text,
  location text,
  membership text not null default 'Social' check (membership in ('Premium', 'Social')),
  role text not null default 'user' check (role in ('user', 'admin')),
  updated_at timestamptz not null default (timezone('utc', now()))
);

alter table public.profiles enable row level security;

create policy "profiles_select_own"
  on public.profiles for select
  using (auth.uid() = id);

create policy "profiles_insert_own"
  on public.profiles for insert
  with check (auth.uid() = id);

create policy "profiles_update_own"
  on public.profiles for update
  using (auth.uid() = id);

-- Promote a user to admin (replace id with your auth.users id from Supabase Dashboard → Authentication):
-- update public.profiles set role = 'admin' where id = '00000000-0000-0000-0000-000000000000';
