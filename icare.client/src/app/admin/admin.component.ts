import { Component } from '@angular/core';
import { RecipeComponent } from '../recipe/recipe.component';
import { IngredientService, Ingredient } from '../services/ingredients.service';
import { MealDbService, TacoApiService, TranslateService } from '../services/apis';
import { Recipe } from '../../models/recipe';
import { RecipeService } from '../services/recipes.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent {
  originalRecipe!: Recipe;
  translatedRecipe!: Recipe;
  loading: boolean = false;
  errorMessage: string = '';

    constructor(private foodService: IngredientService, private taco: TacoApiService, private mealDb: MealDbService, private translate: TranslateService) { }


  //fetchRecipes() {
  //  this.loading = true;
  //  this.errorMessage = '';

  //  const mealId = '52772'; // ID de teste

  //  this.mealDb.getMealById(mealId).subscribe({
  //    next: (recipe) => {
  //      this.originalRecipe = recipe;
  //    },
  //    error: (error) => {
  //      console.error('Erro ao obter receita original:', error);
  //      this.errorMessage = 'Erro ao carregar receita original.';
  //    }
  //  });

  //  this.mealDb.getMealByIdTranslated(mealId).then({
  //    next: (recipe) => {
  //      this.translatedRecipe = recipe;
  //      this.loading = false;
  //    },
  //    error: (error) => {
  //      console.error('Erro ao obter receita traduzida:', error);
  //      this.errorMessage = 'Erro ao carregar receita traduzida.';
  //      this.loading = false;
  //    }
  //  });
  //}


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
    this.mealDb.updateRecipeDB();
  }






}

