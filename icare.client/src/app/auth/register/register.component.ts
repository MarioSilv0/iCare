import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string | null = null;
  passwordErrors: string[] = [];

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((isAuthenticated) => {
      if (isAuthenticated) {
        this.router.navigate(['/home']);
      }
    });
  }

  validatePassword(password: string) {
    this.passwordErrors = []; // Clear errors on every input

    if (!/[a-z]/.test(password)) {
      this.passwordErrors.push('lower'); // Password requires at least one lowercase letter
    } else if (!/[A-Z]/.test(password)) {
      this.passwordErrors.push('upper'); // Password requires at least one uppercase letter
    } else if (!/[0-9]/.test(password)) {
      this.passwordErrors.push('digit'); // Password requires at least one digit
    } else if (!/[^\w\s]/.test(password)) {
      this.passwordErrors.push('nonAlphanumeric'); // Password requires at least one non-alphanumeric character
    }
  }
  passwordIsValide(): boolean {
    return this.passwordErrors.length === 0 && this.password.length >= 8;
  }

  onRegister(): void {
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    const registrationData = { email: this.email, password: this.password };
    this.authService.register(registrationData).subscribe({
      next: (response) => {
        console.log(response);
        console.log('Registration successful');
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error.message;
      },
    });
  }
}
