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
  public isExpanded = false;
  public isLoggedIn: boolean = false;
  constructor(private authService: AuthService, private router: Router) { }
  
  ngOnInit() {
    this.authService.onStateChanged().subscribe((state: boolean) => {
      this.isLoggedIn = state;
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    if (this.isLoggedIn) {
      this.authService.logout();
      this.router.navigateByUrl('login');
    }
  }
}
