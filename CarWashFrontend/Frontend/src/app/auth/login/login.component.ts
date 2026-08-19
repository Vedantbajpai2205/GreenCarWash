import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service'; // adjust the path as needed
import { Router, RouterModule } from '@angular/router';
import { jwtDecode } from 'jwt-decode'; // Make sure to install this package

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule,RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          console.log('Login success:', response);

          // Store token
          localStorage.setItem('token', response.token);

          // Decode the token to get role
          const decodedToken: any = jwtDecode(response.token);
          console.log('Decoded token:', decodedToken);
          // const role = decodedToken.role; // Adjust key if needed
          const role =
            decodedToken[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ];
          
          console.log(role);

          // Redirect based on role
          if (role === 'ADMIN') {
            this.router.navigate(['/admin']);
          } else if (role === 'CUSTOMER') {
            this.router.navigate(['/customer']);
          } else if (role === 'WASHER') {
            this.router.navigate(['/washer']);
          } else {
            console.error('Unknown role:', role);
            this.router.navigate(['/login']);
            // fallback
          }
        },
        error: (error) => {
          alert('Invalid Email Or Password!');
          console.error('Login error:', error);
        },
      });
    }
  }
}
