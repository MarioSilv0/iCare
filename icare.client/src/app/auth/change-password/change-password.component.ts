import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrl: './change-password.component.css'
})
export class ChangePasswordComponent {
  currentPassword = '';
  newPassword = '';
  repeatPassword = '';
  errorMessage: string | undefined;
  passwordErrors: string[] = [];
  showPassword: boolean = false;

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  constructor(private authService: AuthService, private router: Router) { }

  onChange(): void {
    if (!this.newPassword || this.newPassword !== this.repeatPassword) {
      this.errorMessage = 'Invalid New Passwords';
      return;
    }
    const credentials = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword,
    };
    console.log("onChange");
    this.authService.changePassword(credentials).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.log(err);
        this.errorMessage = err.error.message;
;
      },
    });
  }

  validatePassword(password: string) {
    this.passwordErrors = []; // Clear errors on every input

    if (!/[a-z]/.test(password)) {
      this.passwordErrors.push('lower'); // Password requires at least one lowercase letter
    }

    if (!/[A-Z]/.test(password)) {
      this.passwordErrors.push('upper'); // Password requires at least one uppercase letter
    }

    if (!/[0-9]/.test(password)) {
      this.passwordErrors.push('digit'); // Password requires at least one digit
    }

    if (!/[^\w\s]/.test(password)) {
      this.passwordErrors.push('nonAlphanumeric'); // Password requires at least one non-alphanumeric character
    }
  }

  passwordIsValide(): boolean {
    return this.passwordErrors.length === 0 && this.newPassword.length >= 8;
  }
}
