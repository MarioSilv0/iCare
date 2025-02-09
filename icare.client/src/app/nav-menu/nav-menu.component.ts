import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
})

//Mário
export class NavMenuComponent {
  public isExpanded = false;
  public isLoggedIn: boolean = false;
  public commonPath: string = '../../assets/svgs/';
  public extension: string = '.svg';
  public links = [
    {
      icon: `${this.commonPath}home${this.extension}`,
      text: 'Página Principal',
      path: '/',
    },
    {
      icon: `${this.commonPath}user${this.extension}`,
      text: 'Perfil',
      path: '/profile',
    },
    {
      icon: `${this.commonPath}receitas${this.extension}`,
      text: 'Receitas',
      path: '',
    },
    {
      icon: `${this.commonPath}metas${this.extension}`,
      text: 'Meta Alimentar',
      path: '',
    },
    {
      icon: `${this.commonPath}inventory${this.extension}`,
      text: 'Inventário',
      path: '',
    },
    {
      icon: `${this.commonPath}metrics${this.extension}`,
      text: 'Progresso',
      path: '',
    },
    {
      icon: `${this.commonPath}settings${this.extension}`,
      text: 'Configurações',
      path: '',
    },
  ];
  public commandLinks = [
    {
      icon: `${this.commonPath}help${this.extension}`,
      text: 'Ajuda',
      path: '',
    },
    {
      icon: `${this.commonPath}exit${this.extension}`,
      text: 'Terminar Sessão',
      path: '',
    },
  ];

  constructor(private authService: AuthService, private router: Router) {}

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
