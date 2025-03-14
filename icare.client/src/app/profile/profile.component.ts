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

import { Component, OnInit, input } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar'
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { UsersService, User, Permissions } from '../services/users.service';
import { NotificationService, updatedUserNotification, failedToEditEmailUserNotification } from '../services/notifications.service';
import { StorageUtil } from '../utils/StorageUtil';
import { birthdateValidator } from '../utils/Validators';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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

  public profileForm!: FormGroup;
  public todayDate: string;

  public categories: Set<string> = new Set<string>();
  public preferences: Set<string> = new Set<string>();
  public restrictions: Set<string> = new Set<string>();

  constructor(private router: Router, private fb: FormBuilder, private service: UsersService, private snack: MatSnackBar) {
    this.todayDate = new Date().toISOString().split('T')[0];
  }

  ngOnInit() {
    this.setupForm();
    this.getUser();
  }

  get getCategories() {
    return Array.from(this.categories);
  }

  changePicture(file: File): void {
    var reader = new FileReader();
    reader.readAsDataURL(file);

    reader.onload = () => {
      if (typeof reader.result === 'string') this.profileForm.patchValue({ picture: reader.result });
    }
  }

  changeNotifications(value: boolean) {
    this.profileForm.patchValue({ notifications: value });
  }

  addPreference(preference: string) {
    if (!preference || this.preferences.has(preference) || !this.categories.has(preference)) return;

    this.preferences.add(preference);
    this.categories.delete(preference);
    this.showToast("Restrição adicionada com sucesso!", 2000, undefined)
  }

  removePreference(preference: string) {
    this.preferences.delete(preference);
    this.categories.add(preference);
  }

  addRestriction(restriction: string) {
    if (!restriction || this.restrictions.has(restriction) || !this.categories.has(restriction)) return;

    this.restrictions.add(restriction);
    this.categories.delete(restriction);
    this.showToast("Restrição adicionada com sucesso!", 2000, undefined)

  }

  removeRestriction(restriction: string) {
    this.restrictions.delete(restriction);
    this.categories.add(restriction);
    this.showToast("Restrição removida com sucesso!", 2000, undefined)
  }

  setupForm() {
    this.profileForm = this.fb.group({
      picture: [''],
      name: ['', [Validators.required, Validators.minLength(1)]],
      email: ['', [Validators.required, Validators.email]],
      birthdate: ['', [Validators.required, birthdateValidator]],
      height: ['', [Validators.required, Validators.min(0.1), Validators.max(3), Validators.pattern(/^\d*\.?\d+$/)]],
      weight: ['', [Validators.required, Validators.min(0.1), Validators.max(700), Validators.pattern(/^\d*\.?\d+$/)]],
      notifications: [true]
    });
  }

  getUser() {
    this.service.getUser().subscribe(
      (user) => {
        const defaultBirthdate = "2000-01-01";
        const birthdate = (!user.birthdate || user.birthdate === '0001-01-01') ? defaultBirthdate : user.birthdate;

        this.profileForm.patchValue({
          picture: user.picture,
          name: user.name,
          email: user.email,
          birthdate,
          height: user.height,
          weight: user.weight,
          notifications: user.notifications,
        });

        this.preferences = new Set(user.preferences);
        this.restrictions = new Set(user.restrictions);
        this.categories = new Set(user.categories);

        for (const c of user.categories) {
          if (this.preferences.has(c) || this.restrictions.has(c)) this.categories.delete(c);
        }
      },
      (error) => {
        console.error(error);
      }
    );
  }

  updateUser() {
    if (this.profileForm.invalid) {
      console.error("Invalid inputs!"); // Toast
      return;
    }

    const updatedUser = {
      ...this.profileForm.value,
      preferences: Array.from(this.preferences),
      restrictions: Array.from(this.restrictions),
      categories: Array.from(this.categories)
    };
    
    this.service.updateUser(updatedUser).subscribe(
      (user) => {
        NotificationService.showNotification(user.notifications, updatedUserNotification);
        if (user.email !== this.profileForm.value.email) NotificationService.showNotification(user.notifications, failedToEditEmailUserNotification); // Toast

        const permissions: Permissions | null = StorageUtil.getFromStorage('permissions');
        const preferences: boolean = Array.from(user.preferences).length > 0;
        const restrictions: boolean = Array.from(user.restrictions).length > 0;

        const updatedPermissions = { notifications: user.notifications, preferences, restrictions };
        if (!permissions || permissions.notifications !== updatedPermissions.notifications || permissions.preferences !== updatedPermissions.preferences || permissions.restrictions !== updatedPermissions.restrictions) {
          this.service.setPermissions(updatedPermissions);
        }

        this.router.navigate(['/']);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  preventSubmit(event: Event) {
    event.preventDefault();
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
