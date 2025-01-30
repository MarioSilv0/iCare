import { AuthService } from '../auth/auth.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrl: './about.component.css'
})
export class AboutComponent {
  isAdmin: boolean = false;
  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.isAdmin = this.authService.userHasRole('Admin'); // Checa se o usuário é admin
  }
}
