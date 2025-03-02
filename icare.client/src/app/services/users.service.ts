import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

const PROFILE: string = '/api/PublicUser';
const INVENTORY: string = '/api/Inventory';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  constructor(private http: HttpClient) { }

  getUser(): Observable<User> {
    return this.http.get<User>(PROFILE);
  }

  updateUser(user: User): Observable<User> {
    const u = { ...user, preferences: Array.from(user.preferences), restrictions: Array.from(user.restrictions), categories: Array.from(user.categories) }

    return this.http.put<User>(PROFILE, u);
  }

  getInventory(): Observable<Item[]> {
    return this.http.get<Item[]>(INVENTORY);
  }

  updateInventory(items: Item[]): Observable<Item[]> {
    return this.http.put<Item[]>(INVENTORY, items);
  }

  removeInventory(items: string[]): Observable<Item[]> {
    return this.http.delete<Item[]>(INVENTORY, { body: items });
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
  birthdate: string;
  notifications: Boolean;
  height: number;
  weight: number;
  preferences: Set<string>;
  restrictions: Set<string>;
  categories: Set<string>;
}
