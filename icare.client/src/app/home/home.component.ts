import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { UsersService } from '../services/users.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  public isAdmin: boolean = false;
  constructor(private authService: AuthService, private service: UsersService) { }

  ngOnInit() {
    this.isAdmin = this.authService.userHasRole('Admin'); // Checa se o usuário é admin
  
    const userData = localStorage.getItem('user');
    if (userData) return;
    
    this.service.getUser().subscribe(
      (result) => {
        try {
          const simplifiedData = { name: result.name, picture: result.picture, notifications: result.notifications };
          localStorage.setItem('user', JSON.stringify(simplifiedData));
        } catch (e) {
          console.error('Failed to update user data in localStorage:', e);
        }
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
