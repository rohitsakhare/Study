import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';

import { SupabaseService } from '../supabase.service';

export const adminGuard: CanActivateFn = async () => {
  const supabase = inject(SupabaseService);
  const router = inject(Router);

  const { data: sessionData, error: sessionErr } = await supabase.getSession();
  const user = sessionData?.session?.user;

  if (sessionErr || !user) {
    return router.parseUrl('/login');
  }

  const { data: profile, error: profErr } = await supabase.getProfile(user.id);
  if (profErr || profile?.role !== 'admin') {
    return router.parseUrl('/dashboard');
  }

  return true;
};
