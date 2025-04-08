import { Component } from '@angular/core';
import { RecipeComponent } from '../recipe/recipe.component';
import { IngredientService } from '../services/ingredients.service';
import { Ingredient } from '../../models'
import { MealDbService, TacoApiService, TranslateService } from '../services/apis';
import { Recipe } from '../../models/recipe';
import { RecipeService } from '../services/recipes.service';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {
    progressMessage?: string;

  constructor(private foodService: IngredientService, private taco: TacoApiService, private mealDb: MealDbService, private recipeService: RecipeService, private auth:AuthService) { }



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

  getIngredients(meal: any): { ingredient: string, measure: string }[] {
    let ingredients: { ingredient: string, measure: string }[] = [];

    for (let i = 1; i <= 20; i++) {
      const ingredient = meal[`strIngredient${i}`];
      const measure = meal[`strMeasure${i}`];

      if (ingredient && ingredient.trim() !== '') {
        ingredients.push({ ingredient, measure });
      }
    }

    return ingredients;
  }

  updateRecipeDb() {
    this.mealDb.updateRecipeDB(
      (message: string) => {
        this.progressMessage = message;
      }
    );
  }


  /**
   * Deletes all recipes and clears the list.
   */
  deleteAllRecipes(): void {
    if (confirm('Are you sure you want to delete ALL recipes? This action cannot be undone.')) {
      this.recipeService.deleteAllRecipes().subscribe({
        next: (res) => {
          alert('All recipes deleted successfully')
          console.log('All recipes deleted successfully:', res);
        },
        error: (err) => console.error('Error deleting all recipes:', err)
      });
    }
  }

  /**
   * Deletes all test users
   */
  deleteTestUsers(): void {
    if (confirm('Are you sure you want to delete ALL Test Users?')) {
      this.auth.deleteTestUsers().subscribe({
        next: (res) => {
          alert(res.message)
          console.log(res);
        },
        error: (err) => {
          alert(err.message)
          console.error(err)
        }
      });
    }
  }




}

