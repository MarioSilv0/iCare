import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

const URL: string = 'https://localhost:7266/api/';
const PROFILE: string = 'PublicUser/';
const INVENTORY: string = 'Inventory/';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  constructor(private http: HttpClient) { }

  getUser(): Observable<User> {
    return this.http.get<User>(URL + PROFILE);
  }

  updateUser(user: User): Observable<User> {
    return this.http.put<User>(URL + PROFILE, { ...user });
  }

  getInventory(): Observable<Item[]> {
    return this.http.get<Item[]>(URL + INVENTORY);
  }

  updateInventory(items: Item[]): Observable<Item[]> {
    return this.http.put<Item[]>(URL + INVENTORY, items);
  }

  removeInventory(items: string[]): Observable<Item[]> {
    return this.http.delete<Item[]>(URL + INVENTORY, { body: items });
  }
}

export interface Item {
  name: string;
  quantity: number;
  unit: string;
}

export interface User {
  picture: string;
  name: string;
  email: string;
  birthdate: Date;
  notifications: Boolean;
  height: number;
  weight: number;
  preferences: Set<string>;
  restrictions: Set<string>;
  categories: Set<string>;
}
