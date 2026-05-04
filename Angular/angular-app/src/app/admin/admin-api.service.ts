import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { SupabaseService } from '../supabase.service';

export type MembershipTier = 'Premium' | 'Social';
export type AdminUserRole = 'admin' | 'user';

export interface AdminUser {
  id: string;
  email: string | null;
  full_name: string | null;
  bio: string | null;
  avatar_url: string | null;
  contact_number: string | null;
  address: string | null;
  location: string | null;
  membership: MembershipTier;
  role: AdminUserRole;
}

export interface AdminUserUpdatePayload {
  email?: string | null;
  full_name?: string | null;
  bio?: string | null;
  avatar_url?: string | null;
  contact_number?: string | null;
  address?: string | null;
  location?: string | null;
  membership?: MembershipTier;
  role?: AdminUserRole;
}

@Injectable({ providedIn: 'root' })
export class AdminApiService {
  private http = inject(HttpClient);
  private supabase = inject(SupabaseService);

  private async getAccessToken() {
    const { data, error } = await this.supabase.getSession();
    if (error || !data.session?.access_token) {
      throw new Error('You must be signed in.');
    }
    return data.session.access_token;
  }

  async createUser(email: string, password: string): Promise<{ message?: string; id?: string }> {
    const accessToken = await this.getAccessToken();
    return firstValueFrom(
      this.http.post<{ message?: string; id?: string }>(
        '/api/admin/users',
        { email, password },
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      ),
    );
  }

  async getUsers(): Promise<AdminUser[]> {
    const accessToken = await this.getAccessToken();
    return firstValueFrom(
      this.http.get<AdminUser[]>('/api/admin/users', {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }),
    );
  }

  async updateUser(id: string, payload: AdminUserUpdatePayload): Promise<AdminUser> {
    const accessToken = await this.getAccessToken();
    return firstValueFrom(
      this.http.patch<AdminUser>(`/api/admin/users/${id}`, payload, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      }),
    );
  }
}
