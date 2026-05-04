# AngularApp

A small Angular 21 application demonstrating authentication, protected routes, and Supabase-backed user profile management.

## Features

- Angular 21 with standalone components and router-based navigation
- Supabase authentication for sign-in, sign-up, and session management
- Protected `admin` route with `adminGuard`
- Admin user management UI: list users and update details
- `dashboard`, `profile`, and `login` screens
- Profile CRUD using a Supabase `profiles` table
- Server-side rendering support via `@angular/ssr`

## Project structure

- `src/app/login` — login screen and auth flow
- `src/app/dashboard` — application dashboard view
- `src/app/profile` — user profile editor and profile state
- `src/app/admin` — admin-only route protected by guard
- `src/app/supabase.service.ts` — Supabase client and data access helpers
- `supabase/` — SQL schema files for memberships, profiles, and roles

## Setup

1. Install dependencies:

```bash
npm install
```

2. Start the development server:

```bash
npm start
```

3. Open the app in your browser:

```text
http://localhost:4200/
```

## Environment configuration

This project uses `src/environments/environment.ts` for the browser app and a `.env` file for the SSR server.

- `src/environments/environment.ts` is used by the Angular client for anon Supabase access.
- `SUPABASE_SERVICE_ROLE_KEY` must only be provided to the server and must not be exposed to the browser.

Create a `.env` file from `.env.example` and set these values before running the SSR server:

```bash
cp .env.example .env
# then edit .env with your Supabase values
```

Example `.env` contents:

```env
SUPABASE_URL=https://your-project.supabase.co
SUPABASE_ANON_KEY=your-anon-public-key
SUPABASE_SERVICE_ROLE_KEY=your-service-role-key
```

## Build

Build the browser application for production:

```bash
npm run build
```

Build SSR assets and start the server:

```bash
npm run build
npm run serve:ssr:angular-app
```

## Tests

Run unit tests with Vitest:

```bash
npm test
```

## Notes

- The app currently uses Angular CLI v21 with SSR support.
- The Supabase service loads the current user and profile data from the `profiles` table.
- The `admin` route is gated by `adminGuard` and requires the user to have an admin role.

## Resources

- Angular CLI: https://angular.dev/tools/cli
- Supabase JavaScript client: https://supabase.com/docs/reference/javascript
