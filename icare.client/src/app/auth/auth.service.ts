import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = '../api/account';

  constructor(private http: HttpClient) {}

  isAuthenticated(): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/isAuthenticated`, {
      withCredentials: true,
    });
  }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials, {
      withCredentials: true,
    });
  }

  logout(): Observable<any> {
    return this.http.post(
      `${this.baseUrl}/logout`,
      {},
      {
        withCredentials: true,
      }
    );
  }

  register(data: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  resetPassword(data: {
    currentPassword: string;
    newPassword: string;
    repeatPassword: string;
  }): Observable<any> {
    // todo: implement backend
    return this.http.post(`${this.baseUrl}/reset-password`, data);
  }
}
