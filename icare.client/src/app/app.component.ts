import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

//Mário
export class AppComponent {
  showNavMenu: boolean = true;
  showHeader = true;
  constructor(private router: Router) {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const currentRoute = this.router.url;
        if (
          currentRoute.includes('login') ||
          currentRoute.includes('register') ||
          currentRoute.includes('password')
        ) {
          this.showNavMenu = false;
          this.showHeader = false;
        } else {
          this.showNavMenu = true;
          this.showHeader = true;
        }
      });
  }

  title = 'icare.client';
}
