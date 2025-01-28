import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

//Mário
export class LoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string | null = '';
  showPassword: boolean = false;

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    if (this.authService.isLogged()) {
      this.router.navigate(['/home']);
    }
  }

  onLogin(): void {
    const credentials = { email: this.email, password: this.password };
    this.authService.login(credentials).subscribe({
      next: () => {
        this.errorMessage = null;
        this.router.navigate(['/home']);
      },
      error: () => {
        this.errorMessage = 'Invalid login credentials';
      },
    });
  }
}
