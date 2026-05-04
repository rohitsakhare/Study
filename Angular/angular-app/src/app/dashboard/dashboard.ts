import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { SupabaseService } from '../supabase.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class DashboardComponent implements OnInit {
  readonly loading = signal(true);
  readonly logoutLoading = signal(false);
  readonly userEmail = signal<string | null>(null);
  readonly isAdmin = signal(false);

  constructor(
    private supabase: SupabaseService,
    private router: Router,
  ) {}

  async ngOnInit() {
    await this.loadSession();
  }

  private async loadSession() {
    this.loading.set(true);
    try {
      const { data, error } = await this.supabase.getUser();
      if (error || !data.user) {
        await this.router.navigate(['/login']);
        return;
      }
      this.userEmail.set(data.user.email ?? null);

      const { data: profile } = await this.supabase.getProfile(data.user.id);
      this.isAdmin.set(profile?.role === 'admin');
    } finally {
      this.loading.set(false);
    }
  }

  async logout() {
    this.logoutLoading.set(true);
    try {
      const { error } = await this.supabase.signOut();
      if (error) {
        return;
      }
      await this.router.navigate(['/login']);
    } finally {
      this.logoutLoading.set(false);
    }
  }
}
