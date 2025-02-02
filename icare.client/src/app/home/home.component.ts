import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  isAdmin: boolean = false;
  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.isAdmin = this.authService.userHasRole('Admin'); // Checa se o usuário é admin
  }
}
