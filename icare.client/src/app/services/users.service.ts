import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private url: string = 'https://localhost:7266/api/PublicUser/';

  constructor(private http: HttpClient) { }

  getUser(): Observable<User> {
    return this.http.get<User>(this.url);
  }

  updateUser(user: User): Observable<User> {
    return this.http.put<User>(this.url, { ...user });
  }
}
interface Preference {
  id: number;
  name: string;
  isSelected: boolean;
}

interface Restrictions {
  id: number;
  name: string;
  isSelected: boolean;
}

export interface User {
  picture: string;
  name: string;
  email: string;
  birthdate: Date;
  notifications: Boolean;
  height: number;
  weight: number;
  preferences: Preference[];
  restrictions: Restrictions[];
}
