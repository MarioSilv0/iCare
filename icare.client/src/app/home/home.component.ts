import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  public isAdmin: boolean = false;
  constructor(private authService: AuthService, private userService: UsersService) { }

  ngOnInit() {
    this.isAdmin = this.authService.userHasRole('Admin'); // Checa se o usuário é admin

    this.getPermissions();
  }

  getPermissions() {
    if (this.authService.isLogged() && !this.userService.getPermissions()) {
      this.userService.fetchPermissions().subscribe();
    }
  }

  /**
   * Logs out the user and redirects to the login page.
   */
  test() {
    this.authService.logout();
  }
}
