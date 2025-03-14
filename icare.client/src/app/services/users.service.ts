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

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { StorageUtil } from '../utils/StorageUtil';
import { User } from '../../models'

export const PROFILE: string = '/api/User';


/**
 * The `UsersService` class provides methods for interacting with the user profile, inventory, etc.
 */
@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private userSubject = new BehaviorSubject< { picture: string, name: string } | null>(null);
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
   * 
   * This method ensures that `Set` properties (`preferences`, `restrictions`, `categories`) are converted into arrays 
   * for proper serialization before sending the data to the server. Additionally, it emits an update notification 
   * through `userSubject`, allowing other components to react to changes in the user's name and profile picture.
   * 
   * @param {User} user - The updated user object containing new profile details.
   * @returns {Observable<User>} An observable that emits the updated user data upon successful API response.
   */
  updateUser(user: User): Observable<User> {
    const u = { ...user, preferences: Array.from(user.preferences), restrictions: Array.from(user.restrictions), categories: Array.from(user.categories) }

    return this.http.put<User>(PROFILE, u).pipe(
      tap(updatedUser => {
        const currentUser = this.userSubject.value;
        if (!currentUser || currentUser.name !== updatedUser.name || currentUser.picture !== updatedUser.picture)
          this.userSubject.next({ name: updatedUser.name, picture: updatedUser.picture });
      })
    );
  }

  getPreferences(): Observable<string[]> {
    return this.http.get<string[]>(`${PROFILE}/preferences`);
  }

  getRestrictions(): Observable<string[]> {
    return this.http.get<string[]>(`${PROFILE}/restrictions`);
  }
}
