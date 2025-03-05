import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

/// <author>Mário Silva - 202000500</author>
export class AuthService {
  private baseUrl = '/api/account';

  private _authStateChanged: BehaviorSubject<boolean> =
    new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient, private router: Router) {}

  public onStateChanged() {
    return this._authStateChanged.asObservable();
  }

  // Verifica se o token está presente no localStorage
  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  // Armazena o token no localStorage
  private saveToken(token: string, roles: string ): void {
    localStorage.setItem('authToken', token);
    localStorage.setItem('roles', roles);
  }

  // Remove o token do localStorage
  private clearToken(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('roles');
    localStorage.removeItem('permissions');
  }

  // Verifica se está Logged
  public isLogged(): boolean {
    return this.hasToken();
  }

  // Retorna as roles
  public getUserRoles(): string | null {
    return localStorage.getItem('roles');
  }

  public userHasRole(role: string): boolean {
    const roles = this.getUserRoles() ?? '';
    return roles.includes(role);
    //return roles?.includes(role) ?? false;
  }  

  public login(credentials: { email: string; password: string }): Observable<any> {
    return this.http.post<{ token: string, roles:string }>(`${this.baseUrl}/login`, credentials).pipe(map((response) => {
      if (response && response.token) {
        this.saveToken(response.token, response.roles);
        this._authStateChanged.next(true);
      }
    }));
  }

  public googleLogin(idToken: string): Observable<any> {
    return this.http.post<{ token: string, roles: string }>(`${this.baseUrl}/google-login`, { IdToken: idToken }).pipe(map((response) => {
        if (response && response.token) {
          this.saveToken(response.token, response.roles);
          this._authStateChanged.next(true);
        }
      }));
  }

  public logout(): void {
    this.clearToken();
    this._authStateChanged.next(false);
    this.router.navigate(['/login']);
  }

  public register(email: string, password: string ): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, { email, password, clientUrl: window.location.origin });
  }

  public recoverPassword(email: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/recover-password`, { email, clientUrl: window.location.origin });
  }

  public resetPassword(data: { email: string, token: string, newPassword: string }) {
    return this.http.post(`${this.baseUrl}/reset-password`, data);
  }

  public changePassword(data: { currentPassword: string, newPassword: string }) {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    });
    return this.http.post(`${this.baseUrl}/change-password`, data, { headers });
  }

  public confirmEmail(email: string, token: string): Observable<any> {
    return this.http.get(`/api/account/confirm-email?email=${email}&token=${token}`);
  }
}
