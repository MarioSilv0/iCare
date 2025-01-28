import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})

//Mário with 'PeopleAngular(identity)'
export class AuthService {
  private baseUrl = '../api/account';

  private _authStateChanged: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) { }

  public onStateChanged() {
    return this._authStateChanged.asObservable();
  }

  // Verifica se o token está presente no localStorage
  private hasToken(): boolean {
    return !!localStorage.getItem('authToken');
  }

  // Armazena o token no localStorage
  private saveToken(token: string): void {
    localStorage.setItem('authToken', token);
  }

  // Remove o token do localStorage
  private clearToken(): void {
    localStorage.removeItem('authToken');
  }

  public login(credentials: { email: string; password: string }): Observable<any> {
    const res = this.http.post<{ token: string }>(`${this.baseUrl}/login`, credentials).pipe(map((response) => {
      if (response && response.token) {
        this.saveToken(response.token);
        this._authStateChanged.next(true);
      }}));
    return res; 
  }

  public logout(): void {
    this.clearToken();
    this._authStateChanged.next(false);
  }

  public isLogged(): boolean {
    return this.hasToken();
  }

  public register(data: { email: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, data);
  }

  public resetPassword(data: {
    currentPassword: string;
    newPassword: string;
    repeatPassword: string;
  }): Observable<any> {
    // todo: implement backend
    return this.http.post(`${this.baseUrl}/reset-password`, data);
  }
  
  public recover(email: string): Observable<any> {
    //? verificar que tipo de metodo é
    // todo: implement backend
    return this.http.post(`${this.baseUrl}/recover-password`, email);
  }

}
