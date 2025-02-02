import { Component, NgZone, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})

//Mário
export class LoginComponent {
  email = '';
  password = '';
  errorMessage: string | null = null;
  showPassword = false;

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  constructor(private authService: AuthService, private router: Router, private ngZone: NgZone) { }

  ngOnInit(): void {
    const checkGoogle = setInterval(() => {
      if (typeof google !== 'undefined' && google.accounts) {
        clearInterval(checkGoogle);

        google.accounts.id.initialize({
          client_id: '452109114218-ld8o3eiqgar6jg6h42r6q3fvqsevfiv4.apps.googleusercontent.com',
          scope: 'email profile openid',
          callback: this.onGoogleLogin.bind(this),
          context: "signin",
          ux_mode: "popup",
          auto_select: "true",
          itp_support: "true",
        });

        google.accounts.id.renderButton(
          document.getElementById('google-signin'),
          {
            type: "icon",
            shape: "circle",
            theme: "filled_blue",
            text: "signin_with",
            size: "large",
          }
        );
      }
    }, 500);

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
      error: (err) => {
        this.errorMessage = 'Invalid login credentials';
        console.error('login credentials:', err);
      },
    });
  }

  onGoogleLogin(response: any): void {
    console.log('Token de ID recebido:', response.credential);
    this.authService.googleLogin(response.credential).subscribe({
      next: () => {
        this.ngZone.run(() => {
          this.router.navigate(['/home']);
        });
      },
      error: (err) => {
        console.error('Erro ao processar login via Google:', err);
      }
    });
  }

}
