import { Component, OnInit } from '@angular/core';
import { UsersService, User } from '../services/users.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: false,

  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  private id: string = '4362f355-de07-4815-900a-8458338f2ab5';

  public user: User = { name: 'A', email: 'A@example.com', birthdate: new Date(), height: 0, weight: 0 };
  constructor(private router: Router, private service: UsersService) { }

  ngOnInit() {
    this.getUser(this.id);
  }

  getUser(id: string) {
    this.service.getUser(id).subscribe(
      (result) => {
        this.user = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  updateUser() {
    this.service.updateUser(this.id, this.user).subscribe(
      () => {
        this.router.navigate(['/']);
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
