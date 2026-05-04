import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AdminApiService, type AdminUser, type AdminUserUpdatePayload } from './admin-api.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class AdminComponent implements OnInit {
  readonly loading = signal(false);
  readonly listLoading = signal(false);
  readonly updateLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly listError = signal<string | null>(null);
  readonly updateError = signal<string | null>(null);
  readonly successMessage = signal<string | null>(null);
  readonly updateSuccess = signal<string | null>(null);

  readonly users = signal<AdminUser[]>([]);
  readonly selectedUser = signal<AdminUser | null>(null);

  form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
  });

  editForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    role: new FormControl<'admin' | 'user'>('user', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    membership: new FormControl<'Premium' | 'Social'>('Social', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    full_name: new FormControl('', { nonNullable: true }),
    bio: new FormControl('', { nonNullable: true }),
    contact_number: new FormControl('', { nonNullable: true }),
    address: new FormControl('', { nonNullable: true }),
    location: new FormControl('', { nonNullable: true }),
    avatar_url: new FormControl('', { nonNullable: true }),
  });

  constructor(private adminApi: AdminApiService) {}

  ngOnInit() {
    void this.loadUsers();
  }

  async loadUsers() {
    this.listError.set(null);
    this.listLoading.set(true);
    try {
      const users = await this.adminApi.getUsers();
      this.users.set(users);
    } catch (e: unknown) {
      this.listError.set(e instanceof Error ? e.message : 'Failed to load users');
    } finally {
      this.listLoading.set(false);
    }
  }

  async submit() {
    this.errorMessage.set(null);
    this.successMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    try {
      const { email, password } = this.form.getRawValue();
      const res = await this.adminApi.createUser(email, password);
      this.successMessage.set(res.message ?? 'User created.');
      this.form.controls.password.reset('');
      await this.loadUsers();
    } catch (e: unknown) {
      if (e instanceof HttpErrorResponse) {
        const body = e.error as { error?: string } | undefined;
        this.errorMessage.set(body?.error ?? e.message ?? 'Request failed');
        return;
      }
      this.errorMessage.set(e instanceof Error ? e.message : 'Request failed');
    } finally {
      this.loading.set(false);
    }
  }

  selectUser(user: AdminUser) {
    this.selectedUser.set(user);
    this.editForm.patchValue({
      email: user.email ?? '',
      role: user.role,
      membership: user.membership,
      full_name: user.full_name ?? '',
      bio: user.bio ?? '',
      contact_number: user.contact_number ?? '',
      address: user.address ?? '',
      location: user.location ?? '',
      avatar_url: user.avatar_url ?? '',
    });
    this.updateError.set(null);
    this.updateSuccess.set(null);
  }

  clearSelection() {
    this.selectedUser.set(null);
    this.updateError.set(null);
    this.updateSuccess.set(null);
  }

  async submitUpdate() {
    const user = this.selectedUser();
    if (!user) {
      return;
    }

    this.updateError.set(null);
    this.updateSuccess.set(null);

    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.updateLoading.set(true);
    try {
      const raw = this.editForm.getRawValue();
      const payload: AdminUserUpdatePayload = {
        email: raw.email.trim() || null,
        role: raw.role,
        membership: raw.membership,
        full_name: raw.full_name.trim() || null,
        bio: raw.bio.trim() || null,
        contact_number: raw.contact_number.trim() || null,
        address: raw.address.trim() || null,
        location: raw.location.trim() || null,
        avatar_url: raw.avatar_url.trim() || null,
      };

      const updated = await this.adminApi.updateUser(user.id, payload);
      this.updateSuccess.set('User details updated.');
      this.selectedUser.set(updated);
      await this.loadUsers();
    } catch (e: unknown) {
      if (e instanceof HttpErrorResponse) {
        const body = e.error as { error?: string } | undefined;
        this.updateError.set(body?.error ?? e.message ?? 'Request failed');
        return;
      }
      this.updateError.set(e instanceof Error ? e.message : 'Request failed');
    } finally {
      this.updateLoading.set(false);
    }
  }
}
