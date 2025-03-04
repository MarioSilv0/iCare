import { Component } from '@angular/core';
import { AuthService } from './../auth/auth.service';
import { MenuService } from '../services/menu.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  constructor(private authService: AuthService, private menuService: MenuService) {
    this.authService = authService;
  }

  public expand() {
    this.menuService.setMenuState(true);
  }

  public collapse() {
    this.menuService.setMenuState(false);
  }

  public logout() {
    this.authService.logout();
  }
}
