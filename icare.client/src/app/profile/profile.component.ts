import { Component, OnInit } from '@angular/core';
import { UsersService, User } from '../services/users.service';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { NotificationService } from '../services/notifications.service';

@Component({
  selector: 'app-profile',
  standalone: false,

  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  public user: User = {
    picture: '', name: 'A', email: 'A@example.com', birthdate: new Date(), notifications: true, height: 0, weight: 0, preferences: [], restrictions: [] };
  public todayDate: string;

  constructor(private router: Router, private service: UsersService) {
    this.todayDate = new Date().toISOString().split('T')[0];
  }

  ngOnInit() {
    this.getUser();
  }

  getUser() {
    this.service.getUser().subscribe(
      (result) => {
        this.user = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  updateUser() {
    this.service.updateUser(this.user).subscribe(
      () => {
        const notification = {
          permission: this.user.notifications,
          icon: '/assets/svgs/user.svg',
          title: 'Profile Updated',
          body: 'The Information on your profile as been updated successfully!'
        }

        NotificationService.showNotification(notification.permission, notification.icon, notification.title, notification.body);
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
