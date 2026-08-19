import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: unknown, state: unknown): boolean {
    const token = localStorage.getItem('token');

    if (token) {
      // Optionally validate token expiry here
      return true;
    }

    // If not logged in, redirect to login
    this.router.navigate(['/login']);
    return false;
  }
}