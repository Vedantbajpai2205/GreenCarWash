import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../models/login-dto';
import { RegisterDto } from '../models/register-dto';
import { Observable, tap } from 'rxjs';
import {jwtDecode} from 'jwt-decode';
import { json } from 'stream/consumers';

interface DecodedToken {
  fullName: string;
  email: string;
  role: string;
  exp: number;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'http://localhost:5172/api/auth/'; // Adjust as needed

  constructor(private http: HttpClient) {}

  login(dto: LoginDto): Observable<any> {
    return this.http.post(`${this.baseUrl}login`, dto).pipe(
      tap((response: any) => {
        localStorage.setItem('token', response.token);
      })
    );
  }

  register(dto: RegisterDto): Observable<any> {
    return this.http.post(`${this.baseUrl}register`, dto, { responseType: 'text' });
  }

  logout() {
    localStorage.removeItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}