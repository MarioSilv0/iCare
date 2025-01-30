import { Component } from '@angular/core';
import { AuthService } from './../auth/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  isAuth = false;
  authService: AuthService;
  constructor(authService: AuthService) {
    this.authService = authService;
    this.isAuth = this.authService.isLogged();
  }

  logout() {
    this.authService.logout();
  }
}
