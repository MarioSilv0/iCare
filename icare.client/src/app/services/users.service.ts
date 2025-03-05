/**
 * @file Defines the `UsersService` class, which handles HTTP requests related to user profiles
 * and inventory management. This service allows retrieving, updating, and managing user data.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-05
 */

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

const PROFILE: string = '/api/User';
const INVENTORY: string = '/api/Inventory';

/**
 * The `UsersService` class provides methods for interacting with the user profile and inventory.
 * It allows retrieving, updating, and managing user details and inventory items.
 */
@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private userSubject = new BehaviorSubject<User | null>(null);
  // Observable for components to listen for user updates
  public user$ = this.userSubject.asObservable();
  constructor(private http: HttpClient) { }

  /**
   * Retrieves the user's profile data from the API.
   * 
   * @returns {Observable<User>} An observable containing the user profile details.
   */
  getUser(): Observable<User> {
    return this.http.get<User>(PROFILE);
  }

  /**
   * Updates the user's profile data on the API.
   * Converts `Set` properties (preferences, restrictions, categories) into arrays for proper serialization.
   * 
   * @param {User} user - The updated user object.
   * @returns {Observable<User>} An observable containing the updated user data.
   */
  updateUser(user: User): Observable<User> {
    const u = { ...user, preferences: Array.from(user.preferences), restrictions: Array.from(user.restrictions), categories: Array.from(user.categories) }

    return this.http.put<User>(PROFILE, u).pipe(
      // Notify components about the update
      tap(updatedUser => this.userSubject.next(updatedUser))
    );
  }

  /**
   * Retrieves the inventory data from the API.
   * 
   * @returns {Observable<Item[]>} An observable containing an array of inventory items.
   */
  getInventory(): Observable<Item[]> {
    return this.http.get<Item[]>(INVENTORY);
  }

  /**
   * Updates the inventory data on the API.
   * 
   * @param {Item[]} items - The updated inventory list.
   * @returns {Observable<Item[]>} An observable containing the updated inventory items.
   */
  updateInventory(items: Item[]): Observable<Item[]> {
    return this.http.put<Item[]>(INVENTORY, items);
  }

  /**
   * Removes items from the inventory on the API.
   * 
   * @param {string[]} items - The names of the items to remove.
   * @returns {Observable<Item[]>} An observable containing the updated inventory after removal.
   */
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
