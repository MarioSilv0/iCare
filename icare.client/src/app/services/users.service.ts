import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Item } from './api.service';

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
    return this.http.put<User>(PROFILE, { ...user });
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
