import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { UpdateRecipesService } from '../services/update-recipes.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  isAdmin: boolean = false;
  mealsList: any[] = [];

  constructor(private authService: AuthService, private updateRecipesService: UpdateRecipesService) { }

  async ngOnInit() {
      this.isAdmin = this.authService.userHasRole('Admin'); // Checa se o usuário é admin
      await this.updateRecipesService.updateDatabase();
      this.mealsList = this.updateRecipesService.mealsList;
  }
}
