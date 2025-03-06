/**
 * @file Defines the `ProfileComponent` class, responsible for managing user profile data.
 * It allows users to view and edit their profile details, including preferences, restrictions, and profile picture.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Component, OnInit } from '@angular/core';
import { UsersService, User, Permissions } from '../services/users.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { NotificationService, updatedUserNotification, failedToEditEmailUserNotification } from '../services/notifications.service';
import { StorageUtil } from '../utils/StorageUtil';

@Component({
  selector: 'app-profile',
  standalone: false,

  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})

/**
  * The `ProfileComponent` class manages user profile details, allowing the user to update their personal information,
  * manage dietary preferences and restrictions, and change their profile picture.
  */
export class ProfileComponent implements OnInit {
  public user: User = {
    picture: '', name: 'Loading', email: '...', birthdate: "2000-01-01", notifications: true, height: 0, weight: 0, preferences: new Set(), restrictions: new Set(), categories: new Set() };
  public todayDate: string;

  constructor(private router: Router, private service: UsersService) {
    this.todayDate = new Date().toISOString().split('T')[0];
  }

  /**
   * Initializes the component and retrieves user data.
   */
  ngOnInit() {
    this.getUser();
  }

  /**
   * Adds a new preference to the user's profile and removes it from the categories list.
   * @param {Event} event - The event triggered when selecting a preference.
   */
  addPreference(event: Event) {
    const target = event.target as HTMLSelectElement;
    const preference = target.value;
    if (!preference || this.user.preferences.has(preference)) {
      target.value = '';
      return;
    }

    this.user.preferences.add(preference);
    this.user.categories.delete(preference);
    target.value = "";
  }

  /**
   * Removes a preference from the user's profile and adds it back to the categories list.
   * @param {string} preference - The preference to remove.
   */
  removePreference(preference: string) {
    this.user.preferences.delete(preference);
    this.user.categories.add(preference);
  }

  /**
   * Adds a new dietary restriction to the user's profile and removes it from the categories list.
   * @param {Event} event - The event triggered when selecting a restriction.
   */
  addRestriction(event: Event) {
    const target = event.target as HTMLSelectElement;
    const restriction = target.value;
    if (!restriction || this.user.restrictions.has(restriction)) {
      target.value = "";
      return;
    }

    this.user.restrictions.add(restriction);
    this.user.categories.delete(restriction);
    target.value = "";
  }

  /**
   * Removes a dietary restriction from the user's profile and adds it back to the categories list.
   * @param {string} restriction - The restriction to remove.
   */
  removeRestriction(restriction: string) {
    this.user.restrictions.delete(restriction);
    this.user.categories.add(restriction);
  }

  /**
   * Retrieves the user's profile data from the backend service.
   * If the birthdate is missing, it defaults to `"2000-01-01"`.
   * It also populates the preferences and restrictions.
   */
  getUser() {
    this.service.getUser().subscribe(
      (result) => {
        let birthdate = (!result.birthdate || result.birthdate === '0001-01-01') ? this.user.birthdate : result.birthdate;
        this.user = { ...result, birthdate, categories: new Set(result.categories), preferences: new Set(result.preferences), restrictions: new Set(result.restrictions) };

        for (const c of this.user.categories) {
          if (this.user.preferences.has(c) || this.user.restrictions.has(c)) this.user.categories.delete(c);
        }
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Updates the user's profile data and displays notifications based on the result.
   * If the email update fails, an additional notification is shown.
   * Updates local storage if there are changes.
   */
  updateUser() {
    this.service.updateUser(this.user).subscribe(
      (result) => {
        NotificationService.showNotification(this.user.notifications, updatedUserNotification);
        if (result.email !== this.user.email) NotificationService.showNotification(this.user.notifications, failedToEditEmailUserNotification);

        const permissions: Permissions | null = StorageUtil.getFromStorage('permissions');
        const preferences = Array.from(result.preferences);
        const restrictions = Array.from(result.restrictions);

        const updatedUser = { notifications: result.notifications, preferences: preferences.length > 0, restrictions: restrictions.length > 0 };

        if (!permissions || permissions.notifications !== updatedUser.notifications || permissions.preferences !== updatedUser.preferences || permissions.restrictions !== updatedUser.restrictions) {
          StorageUtil.saveToStorage('permissions', updatedUser);
        }
        
        this.router.navigate(['/']);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Handles profile picture selection and updates the `user.picture` property.
   * Ensures only image files are accepted.
   * @param {Event | null} event - The file input change event.
   */
  onSelectFile(event: Event | null): void {
    if (!event) return;

    const input = event.target as HTMLInputElement;

    if (input.files && input.files[0]) {
      const file = input.files[0];
      if (!file.type.startsWith('image/')) return;

      var reader = new FileReader();
      reader.readAsDataURL(file);

      reader.onload = () => {
        if (typeof reader.result === 'string') this.user.picture = reader.result;
      }
    }
  }

  /**
   * Navigates the user to the password change page.
   */
  changePassword(): void {
    this.router.navigate(['/change-password']);
  }
}
