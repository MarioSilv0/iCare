import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { PasswordValidatorService } from '../password-validator.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  currentPassword = '';
  newPassword = '';
  repeatPassword = '';
  errorMessage: string = '';
  passwordErrorMessage: string = '';
  isPasswordValid: boolean = false;
  showPassword: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private passwordValidator: PasswordValidatorService
  ) { }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  validateNewPassword(): void {
    const validation = this.passwordValidator.validate(this.newPassword);
    this.isPasswordValid = validation.valid;
    this.passwordErrorMessage = validation.message || '';
  }

  onChange(): void {
    if (!this.newPassword || this.newPassword !== this.repeatPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }
    this.validateNewPassword();
    if (!this.isPasswordValid) {
      this.errorMessage = 'Password is too weak';
      return;
    }

    const credentials = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword
    };

    this.authService.changePassword(credentials).subscribe({
      next: () => {
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error( err);
        this.errorMessage = err.error.message;
      }
    });
  }
}
