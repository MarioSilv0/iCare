import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { PasswordValidatorService } from '../password-validator.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent {
  email: string = '';
  token: string = '';
  newPassword: string = '';
  repeatPassword = '';
  isPasswordValid = false;
  passwordErrorMessage = '';
  errorMessage = '';
  showPassword = false;

  constructor(private authService: AuthService, private router: Router, private passwordValidator: PasswordValidatorService) {}

  ngOnInit(): void {
    const urlParams = new URLSearchParams(window.location.search);
    this.email = urlParams.get('email') || '';
    this.token = urlParams.get('token') || '';
  }

  validateNewPassword() {
    const validation = this.passwordValidator.validate(this.newPassword);
    this.isPasswordValid = validation.valid;
    this.passwordErrorMessage = validation.message || '';
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onReset(): void {
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
      email: this.email,
      token: this.token,
      newPassword: this.newPassword,
    };
    this.authService.resetPassword(credentials).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.log(err)
        this.errorMessage = err.error.message;
      },
    });
  }
}
