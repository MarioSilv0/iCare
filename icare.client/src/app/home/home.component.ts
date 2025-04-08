import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { PermissionsService } from '../services/permissions.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  public isAdmin: boolean = false;
  constructor(private authService: AuthService, private permissionsService: PermissionsService) { }

  ngOnInit() {
    this.isAdmin = this.authService.userHasRole('Admin'); 

    this.getPermissions();
  }

  getPermissions() {
    if (!this.permissionsService.getPermissions()) {
      this.permissionsService.fetchPermissions().subscribe();
    }
  }

  /**
   * Logs out the user and redirects to the login page.
   */
  test() {
    this.authService.logout();
  }
}
