import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

  //Mário
export class AuthService {
  private baseUrl = '../api/account';

  constructor(private http: HttpClient) { }

  login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials, {
      withCredentials: true,
    });
  }

  logout(): Observable<any> {
    return this.http.post(`${this.baseUrl}/logout`, {}, {
      withCredentials: true,
    });
  }

  isAuthenticated(): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/isAuthenticated`, {
      withCredentials: true,
    });
  }
}
