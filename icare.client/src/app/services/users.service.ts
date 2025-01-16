import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  private url: string = 'https://localhost:7266/';

  constructor(private http: HttpClient) { }

  getUser(id: string): Observable<User> {
    return this.http.get<User>(this.url + id);
  }

  updateUser(user: User): Observable<User> {
    console.log('UpdateUser111 called with:', user, user.id);
    return this.http.put<User>(this.url + user.id, user);
  }
}

export interface User {
  id: string;
  name: string;
  email: string;
  birthdate: Date;
  height: number;
  weight: number;
}
