import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

  //Mário
export class AppComponent {
  showNavMenu: boolean = true;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      const currentRoute = this.router.url;
      if (currentRoute.includes('login') || currentRoute.includes('register') || currentRoute.includes('password')) {
        this.showNavMenu = false;
      } else {
        this.showNavMenu = true;
      }
    });
  }

  title = 'icare.client';
}
