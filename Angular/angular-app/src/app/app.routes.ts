import { Routes } from '@angular/router';

import { adminGuard } from './admin/admin.guard';
import { AdminComponent } from './admin/admin';
import { DashboardComponent } from './dashboard/dashboard';
import { LoginComponent } from './login/login';
import { ProfileComponent } from './profile/profile';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'admin', component: AdminComponent, canActivate: [adminGuard] },
];
