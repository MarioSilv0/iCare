import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { PasswordValidatorService } from '../password-validator.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  passwordErrorMessage: string = '';
  isPasswordValid: boolean = false;
  showPassword: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private passwordValidator: PasswordValidatorService
  ) { }

  ngOnInit(): void {
      if (this.authService.isLogged()) {
        this.router.navigate(['/login']);
      }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  validatePassword(): void {
    const validation = this.passwordValidator.validate(this.password);
    this.isPasswordValid = validation.valid;
    this.passwordErrorMessage = validation.message || '';
  }

  onRegister(): void {
    if (!this.password || this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }
    this.validatePassword();
    if (!this.isPasswordValid) {
      this.errorMessage = 'Password is too weak';
      return;
    }

    this.authService.register(this.email, this.password).subscribe({
      next: () => {
        this.router.navigate(['/login']);
        alert("User Registed.\nPlease check your email to confirm your registration.");
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error.message;
      },
    });
  }
}
