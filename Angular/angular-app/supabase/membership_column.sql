-- Run once in Supabase SQL Editor if `profiles` exists without `membership`.

alter table public.profiles
  add column if not exists membership text;

update public.profiles
set membership = 'Social'
where membership is null or membership not in ('Premium', 'Social');

alter table public.profiles
  alter column membership set default 'Social';

alter table public.profiles
  alter column membership set not null;

alter table public.profiles
  drop constraint if exists profiles_membership_check;

alter table public.profiles
  add constraint profiles_membership_check check (membership in ('Premium', 'Social'));
