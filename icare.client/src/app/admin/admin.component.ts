import { Component } from '@angular/core';
import { RecipeComponent } from '../recipe/recipe.component';
import { TacoApiService } from '../services/taco-api.service';
import { IngredientService, Ingredient } from '../services/ingredients.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {

  constructor(private foodService: IngredientService, private taco: TacoApiService) { }

  updateIngredientDb() {
    this.taco.getAllFood().subscribe(
      (ingredients: Ingredient[]) => {
        this.foodService.updateDB(ingredients).subscribe(
          () => {
            console.log('Database updated successfully!')
            alert('Database updated successfully')
          },
          (error) => console.error('Error updating database:', error)
        );
      },
      (error) => console.error('Error fetching ingredients:', error)
    );
  }



}

