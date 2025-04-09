import { Component, NgZone } from '@angular/core';
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
  emailError: string = ""
  password = '';
  passError: string = ""
  errorMessage: string | null = null;
  showPassword = false;
  private readonly EMAIL_REGEX_PATTERN = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$" // formato valido de email com @ e .
  private readonly PASSWORD_REGEX_PATTERN = "^(?=.*[A-Za-z])(?=.*\\d).{8,}$" // 8 caracteres sem espaços, 1 letra minuscula e maiuscula

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
            type: "standard",
            shape: "pill",
            theme: "filled_blue",
            text: "continue_with",
            size: "large",
            logo_alignment:"center"
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
        console.error('login credentials:', err.message);
      },
    });
  }

  onGoogleLogin(response: any): void {
    this.authService.googleLogin(response.credential).subscribe({
      next: () => {
        this.ngZone.run(() => {
          this.router.navigate(['/home']);
        });
      },
      error: (err) => {
        console.error('Erro ao processar login via Google:', err.message);
      }
    });
  }

  validateEmail() {
    this.emailError = ""
    const reg = new RegExp(this.EMAIL_REGEX_PATTERN)
    if (!reg.test(this.email)) this.emailError = "O email deve seguir o formato \"john@doe.com\""
    else this.emailError = ""
  }

  validatePassword() {
    this.passError = ""
    const reg = new RegExp(this.PASSWORD_REGEX_PATTERN)
    console.log({ pass: this.password, valid: reg.test(this.password) })
    if (!reg.test(this.password)) this.passError = "A palavra passe deve ter no minimo 8 caracteres (letras e números)."
    else this.passError = ""
  }
}
