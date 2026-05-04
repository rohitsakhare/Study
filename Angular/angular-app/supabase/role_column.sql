-- Run once if `profiles` exists without `role`.

alter table public.profiles
  add column if not exists role text;

update public.profiles
set role = 'user'
where role is null or role not in ('user', 'admin');

alter table public.profiles
  alter column role set default 'user';

alter table public.profiles
  alter column role set not null;

alter table public.profiles
  drop constraint if exists profiles_role_check;

alter table public.profiles
  add constraint profiles_role_check check (role in ('user', 'admin'));
