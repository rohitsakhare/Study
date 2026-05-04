import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { SupabaseService } from '../supabase.service';
import type { MembershipTier, UserProfile } from './profile.types';

function optionalUrlValidator(control: AbstractControl): ValidationErrors | null {
  const raw = control.value;
  if (raw == null || String(raw).trim() === '') {
    return null;
  }
  try {
    new URL(String(raw).trim());
    return null;
  } catch {
    return { url: true };
  }
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterLink, DatePipe, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class ProfileComponent implements OnInit {
  readonly membershipOptions: { value: MembershipTier; label: string }[] = [
    { value: 'Premium', label: 'Premium' },
    { value: 'Social', label: 'Social' },
  ];

  readonly loading = signal(true);
  readonly profile = signal<UserProfile | null>(null);
  readonly authEmail = signal<string | null>(null);
  readonly loadError = signal<string | null>(null);
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  private userId: string | null = null;

  profileForm = new FormGroup({
    full_name: new FormControl('', { nonNullable: true }),
    bio: new FormControl('', { nonNullable: true }),
    contact_number: new FormControl('', { nonNullable: true }),
    address: new FormControl('', { nonNullable: true }),
    location: new FormControl('', { nonNullable: true }),
    avatar_url: new FormControl('', {
      nonNullable: true,
      validators: [optionalUrlValidator],
    }),
    membership: new FormControl<MembershipTier>('Social', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  constructor(
    private supabase: SupabaseService,
    private router: Router,
  ) {}

  async ngOnInit() {
    await this.load();
  }

  private async load() {
    this.loading.set(true);
    this.loadError.set(null);
    try {
      const { data: authData, error: authError } = await this.supabase.getUser();
      if (authError || !authData.user) {
        await this.router.navigate(['/login']);
        return;
      }

      this.userId = authData.user.id;
      const email = authData.user.email ?? null;
      this.authEmail.set(email);

      const { data: row, error: profileError } = await this.supabase.getProfile(
        authData.user.id,
      );
      if (profileError) {
        this.loadError.set(profileError.message);
        return;
      }
      this.profile.set(row);
      this.patchFormFromProfile(row);
      this.editing.set(false);
    } finally {
      this.loading.set(false);
    }
  }

  private patchFormFromProfile(p: UserProfile | null) {
    const tier: MembershipTier =
      p?.membership === 'Premium' || p?.membership === 'Social' ? p.membership : 'Social';
    this.profileForm.patchValue({
      full_name: p?.full_name ?? '',
      bio: p?.bio ?? '',
      contact_number: p?.contact_number ?? '',
      address: p?.address ?? '',
      location: p?.location ?? '',
      avatar_url: p?.avatar_url ?? '',
      membership: tier,
    });
  }

  private toPayload(): {
    full_name: string | null;
    bio: string | null;
    contact_number: string | null;
    address: string | null;
    location: string | null;
    avatar_url: string | null;
    membership: MembershipTier;
  } {
    const v = this.profileForm.getRawValue();
    const trimOrNull = (s: string) => {
      const t = s.trim();
      return t === '' ? null : t;
    };
    const m = v.membership;
    const membership: MembershipTier = m === 'Premium' || m === 'Social' ? m : 'Social';
    return {
      full_name: trimOrNull(v.full_name),
      bio: trimOrNull(v.bio),
      contact_number: trimOrNull(v.contact_number),
      address: trimOrNull(v.address),
      location: trimOrNull(v.location),
      avatar_url: trimOrNull(v.avatar_url),
      membership,
    };
  }

  startEdit() {
    this.saveError.set(null);
    this.patchFormFromProfile(this.profile());
    this.editing.set(true);
  }

  cancelEdit() {
    this.saveError.set(null);
    this.patchFormFromProfile(this.profile());
    this.editing.set(false);
  }

  async save() {
    this.saveError.set(null);
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }
    if (!this.userId) {
      return;
    }

    this.saving.set(true);
    try {
      const { data, error } = await this.supabase.upsertProfile(this.userId, this.toPayload());
      if (error) {
        this.saveError.set(error.message);
        return;
      }
      if (data) {
        this.profile.set(data);
      }
      this.editing.set(false);
    } finally {
      this.saving.set(false);
    }
  }
}
