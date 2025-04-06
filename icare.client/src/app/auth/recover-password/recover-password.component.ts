import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-recover-password',
  templateUrl: './recover-password.component.html',
  styleUrl: './recover-password.component.css',
})
export class RecoverPasswordComponent {
  email: string = '';
  errorMessage: string | undefined;
  private readonly EMAIL_REGEX_PATTERN = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$" // formato valido de email com @ e .

  constructor(private authService: AuthService, private router: Router) {}

  ngInit() {}

  onRecover() {
    this.errorMessage = '';
    this.authService.recoverPassword(this.email).subscribe({
      next: () => {
        console.log('Recover successful');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        console.error(err);
        if (!this.email) this.errorMessage = 'Email cant be empty.';
        else this.errorMessage = 'Invalid Email.';
      },
    });
  }

  validateEmail() {
    this.errorMessage = ""
    const reg = new RegExp(this.EMAIL_REGEX_PATTERN)
    if (!reg.test(this.email)) this.errorMessage = "O email deve seguir o formato \"john@doe.com\""
    else this.errorMessage = ""
  }

  onReset() {
    this.email = '';
    this.errorMessage = '';
  }
}
