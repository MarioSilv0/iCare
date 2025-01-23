import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { inject } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
})

//Mário
export class NavMenuComponent {
  isExpanded = false;

  authService = inject(AuthService);
  router = inject(Router);
  isLoggedIn = this.authService.isAuthenticated().subscribe((isAuthenticated: boolean) => isAuthenticated) || false;


  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => console.error('Logout failed', err),
    });
  }
}
