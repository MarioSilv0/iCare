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

  onReset() {
    this.email = '';
    this.errorMessage = '';
  }
}
