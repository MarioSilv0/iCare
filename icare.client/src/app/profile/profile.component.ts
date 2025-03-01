import { Component, OnInit } from '@angular/core';
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
export class ProfileComponent implements OnInit {
  public user: User = {
    picture: '', name: 'Loading', email: '...', birthdate: new Date("01/01/2000"), notifications: true, height: 0, weight: 0, preferences: new Set(), restrictions: new Set(), categories: new Set() };
  public todayDate: string;

  public availablePreferences: Set<string> = new Set();
  public availableRestrictions: Set<string> = new Set();

  constructor(private router: Router, private service: UsersService) {
    this.todayDate = new Date().toISOString().split('T')[0];
  }

  ngOnInit() {
    this.getUser();
  }

  addPreference(event: Event) {
    const target = event.target as HTMLSelectElement;
    const preference = target.value;
    if (!preference) return;

    this.user.preferences.add(preference);
    this.availablePreferences.delete(preference);

    target.value = "";
  }

  removePreference(preference: string) {
    this.user.preferences.delete(preference);
    this.availablePreferences.add(preference);
  }

  addRestriction(event: Event) {
    const target = event.target as HTMLSelectElement;
    const restriction = target.value;
    if (!restriction) return;

    this.user.restrictions.add(restriction);
    this.availableRestrictions.delete(restriction);

    target.value = "";
  }

  removeRestriction(restriction: string) {
    this.user.restrictions.delete(restriction);
    this.availableRestrictions.add(restriction);
  }

  getUser() {
    this.service.getUser().subscribe(
      (result) => {
        this.user = { ...result, categories: new Set(result.categories), preferences: new Set(result.preferences), restrictions: new Set(result.restrictions)};

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
          }
        } catch (e) {
          console.error('Failed to update user data in localStorage:', e);
        }
        
        this.router.navigate(['/']);
      },
      (error) => {
        console.error(error);
      }
    );
  }

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

  changePassword(): void {
    this.router.navigate(['/change-password']);
  }
}
