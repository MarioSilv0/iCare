import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { MenuService } from './services/menu.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})

//Mário
export class AppComponent {
  showHeader = true;
  showNavMenu = true;
  isExpanded = false;
  constructor(private menuService: MenuService, private router: Router) {
    this.menuService.showNavMenu$.subscribe(state => {this.isExpanded = state})

    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe(() => {
        const currentRoute = this.router.url;
        if (
          currentRoute.includes('login') ||
          currentRoute.includes('register') ||
          currentRoute.includes('password')
        ) {
          this.showHeader = false;
          this.showNavMenu = false;
        } else {
          this.showHeader = true;
          this.showNavMenu = true;
        }
      });
  }

  title = 'icare.client';
}
