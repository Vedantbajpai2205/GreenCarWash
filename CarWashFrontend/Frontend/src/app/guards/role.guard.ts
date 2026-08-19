// role.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const expectedRole = route.data['role'];
    const token = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/login']);
      return false;
    }

    const payload = JSON.parse(atob(token.split('.')[1]));
    const userRole = payload[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ];

    if (userRole !== expectedRole) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }
}
