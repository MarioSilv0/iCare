import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent {
  currentPassword = '';
  newPassword = '';
  repeatPassword = '';
  errorMessage: string | undefined;
  passwordErrors: string[] = [];

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {}

  onReset(): void {
    const credentials = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword,
      repeatPassword: this.repeatPassword,
    };
    this.authService.resetPassword(credentials).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = 'Passwords dont match.';
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
