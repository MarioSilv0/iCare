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
import { MatSnackBar } from '@angular/material/snack-bar'
import { UsersService, User } from '../services/users.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { NotificationService, updatedUserNotification, failedToEditEmailUserNotification } from '../services/notifications.service';

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

  public availablePreferences: Set<string> = new Set();
  public availableRestrictions: Set<string> = new Set();

  constructor(private router: Router, private service: UsersService, private snack: MatSnackBar) {
    this.todayDate = new Date().toISOString().split('T')[0];
  }

  /**
   * Initializes the component and retrieves user data.
   */
  ngOnInit() {
    this.getUser();
  }

  /**
   * Adds a new preference to the user's profile and removes it from the available preferences list.
   * @param {Event} event - The event triggered when selecting a preference.
   */
  addPreference(event: Event) {
    const target = event.target as HTMLSelectElement;
    const preference = target.value;
    if (!preference) return;

    this.user.preferences.add(preference);
    this.availablePreferences.delete(preference);
    this.showToast("Preferência adicionada com sucesso!", 2000, undefined)

    target.value = "";
  }

  /**
   * Removes a preference from the user's profile and adds it back to the available preferences list.
   * @param {string} preference - The preference to remove.
   */
  removePreference(preference: string) {
    this.user.preferences.delete(preference);
    this.availablePreferences.add(preference);
    this.showToast("Preferência removida com sucesso!", 2000, undefined)
  }

  /**
   * Adds a new dietary restriction to the user's profile and removes it from the available restrictions list.
   * @param {Event} event - The event triggered when selecting a restriction.
   */
  addRestriction(event: Event) {
    const target = event.target as HTMLSelectElement;
    const restriction = target.value;
    if (!restriction) return;

    this.user.restrictions.add(restriction);
    this.availableRestrictions.delete(restriction);
    this.showToast("Restrição adicionada com sucesso!", 2000, undefined)

    target.value = "";
  }

  /**
   * Removes a dietary restriction from the user's profile and adds it back to the available restrictions list.
   * @param {string} restriction - The restriction to remove.
   */
  removeRestriction(restriction: string) {
    this.user.restrictions.delete(restriction);
    this.availableRestrictions.add(restriction);
    this.showToast("Restrição removida com sucesso!", 2000, undefined)
  }

  /**
   * Retrieves the user's profile data from the backend service.
   * If the birthdate is missing, it defaults to `"2000-01-01"`.
   * It also populates the available preferences and restrictions.
   */
  getUser() {
    this.service.getUser().subscribe(
      (result) => {
        let birthdate = (!result.birthdate || result.birthdate === '0001-01-01') ? this.user.birthdate : result.birthdate;
        this.user = { ...result, birthdate, categories: new Set(result.categories), preferences: new Set(result.preferences), restrictions: new Set(result.restrictions) };

        for (const c of this.user.categories) {
          if (!this.user.preferences.has(c)) this.availablePreferences.add(c);
          if (!this.user.restrictions.has(c)) this.availableRestrictions.add(c);
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

        try {
          const storedUser = localStorage.getItem('user');
          const parsedUser = storedUser ? JSON.parse(storedUser) : null;

          const updatedUser = { name: result.name, picture: result.picture, notifications: result.notifications };

          if (!parsedUser || parsedUser.name !== updatedUser.name || parsedUser.picture !== updatedUser.picture || parsedUser.notifications !== updatedUser.notifications) {
            localStorage.setItem('user', JSON.stringify(updatedUser));
            this.showToast("Dados atualizados com sucesso!", 2000, undefined)
          }
        } catch (e) {
          console.error('Failed to update user data in localStorage:', e);
          this.showToast("Erro ao atualizar dados!", 2000, undefined, "fail-snackbar")
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
        if (typeof reader.result === 'string') {
          this.user.picture = reader.result
          this.showToast("Alterou a imagem com sucesso!", 2000, undefined)
        };
      }
    }
  }

  /**
   * Navigates the user to the password change page.
   */
  changePassword(): void {
    this.router.navigate(['/change-password']);
  }

  showToast(message: string, duration: 2000, action: string | undefined, ownClass?: string) {
    let classToUse = ownClass ? ownClass : 'success-snackbar';
    setTimeout(() => {
      this.snack.open(message, action, {
        duration: duration,
        panelClass: [classToUse]
      })
    }, duration)
  }
}
