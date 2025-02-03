import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private url: string = 'https://localhost:7266/api/PublicUser/';

  constructor(private http: HttpClient) { }

  getUser(token: string): Observable<User> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<User>(this.url, { headers });
  }

  updateUser(id: string, user: User): Observable<User> {
    return this.http.put<User>(this.url + id, { ...user, id });
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
