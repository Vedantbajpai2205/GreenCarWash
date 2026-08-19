import { Routes } from '@angular/router';
// import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard'; // You need to create this guard
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent) },

  {
  path: 'customer',
  loadComponent: () => import('./auth/customer/customer.component').then(m => m.CustomerComponent),
  canActivate: [AuthGuard, RoleGuard],
  data: { role: 'CUSTOMER' }
},
{
  path: 'admin',
  loadComponent: () => import('./auth/admin/admin.component').then(m => m.AdminComponent),
  canActivate: [AuthGuard, RoleGuard],
  data: { role: 'ADMIN' }
},
{
  path: 'washer',
  loadComponent: () => import('./auth/washer/washer.component').then(m => m.WasherComponent),
  canActivate: [AuthGuard, RoleGuard],
  data: { role: 'WASHER' }
},
{
   path: '', 
   redirectTo: 'login', 
   pathMatch: 'full' }
];
