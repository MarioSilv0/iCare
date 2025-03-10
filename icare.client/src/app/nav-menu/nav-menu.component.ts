/**
 * @file Defines the `NavMenuComponent` class, responsible for managing the navigation menu.
 * It handles menu expansion, user authentication state, and navigation links.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { MenuService } from '../services/menu.service';
import { UsersService, User } from '../services/users.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
})

/**
  * The `NavMenuComponent` class manages the navigation menu, including user authentication state,
  * menu expansion/collapse functionality, and available navigation links.
  */
export class NavMenuComponent {
  public isExpanded: boolean = false;
  public isLoggedIn: boolean = false;
  public username: string | null = null;
  public picture: string | null = null;
  public commonPath: string = '../../assets/svgs/';
  public extension: string = '.svg';
  public links = [
    {
      icon: `${this.commonPath}home${this.extension}`,
      text: 'Página Principal',
      path: '/home',
    },
    {
      icon: `${this.commonPath}user${this.extension}`,
      text: 'Perfil',
      path: '/profile',
    },
    {
      icon: `${this.commonPath}receitas${this.extension}`,
      text: 'Receitas',
      path: '/recipes',
    },
    {
      icon: `${this.commonPath}metas${this.extension}`,
      text: 'Meta Alimentar',
      path: '/goal',
    },
    {
      icon: `${this.commonPath}inventory${this.extension}`,
      text: 'Inventário',
      path: '/inventory',
    },
    {
      icon: `${this.commonPath}metrics${this.extension}`,
      text: 'Progresso',
      path: '/progress',
    },
    {
      icon: `${this.commonPath}settings${this.extension}`,
      text: 'Configurações',
      path: '/settings',
    },
    {
      icon: `${this.commonPath}help${this.extension}`,
      text: 'Ajuda',
      path: '/help',
    },
    {
      icon: `${this.commonPath}exit${this.extension}`,
      text: 'Sign Out',
      path: '/',
    },
  ];
  constructor(private authService: AuthService, private menuService: MenuService, private userService: UsersService) {
    this.menuService.showNavMenu$.subscribe(showNav => {
      this.isExpanded = showNav
    })
  }

  /**
   * Initializes the component, subscribes to authentication state changes, and retrieves user data from local storage.
   */
  ngOnInit() {
    this.authService.onStateChanged().subscribe((state: boolean) => {
      this.isLoggedIn = state;
    });

    this.getInfo();
  }

  getInfo() {
    if (!this.authService.isLogged()) return;
    this.userService.getUser().subscribe(user => {
      this.username = user.name;
      this.picture = user.picture;
    });

    this.userService.user$.subscribe(user => {
      if (user === null) return;

      this.username = user.name;
      this.picture = user.picture;
    });
  }

  /**
   * Collapses the navigation menu.
   */
  collapse() {
    this.isExpanded = false
    this.menuService.setMenuState(this.isExpanded)
  }

  /**
   * Toggles the navigation menu state and notifies the `MenuService` of the change.
   */
  toggle() {
    this.isExpanded = !this.isExpanded;
    this.menuService.setMenuState(this.isExpanded)
  }

  /**
   * Logs out the user if they are currently logged in.
   */
  logout() {
    if (this.isLoggedIn) {
      this.authService.logout();
    }
  }
}
