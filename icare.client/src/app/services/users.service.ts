import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private url: string = 'https://localhost:7266/api/PublicUser/Edit/';

  constructor(private http: HttpClient) { }

  getUser(id: string): Observable<User> {
    return this.http.get<User>(this.url + id);
  }

  updateUser(id: string, user: User): Observable<User> {
    return this.http.put<User>(this.url + id, { ...user, id });
  }
}

export interface User {
  picture: string;
  name: string;
  email: string;
  birthdate: Date;
  height: number;
  weight: number;
}
