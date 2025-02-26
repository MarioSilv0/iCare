import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrl: './recipe.component.css',
})
export class RecipeComponent {
  public url: string =
    'https://www.themealdb.com/api/json/v1/1/lookup.php?i=52772';
  public recipe: Recipe | null = null;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.url = window.location.toString().trim();
    // fetch from server and set the recipe
    // this.recipe = this.http.get<Recipe>(this.url);
    this.recipe = {
      id: 52772,
      title: 'Teriyaki Chicken Casserole',
      instructions:
        'Preheat oven to 350°. In a large skillet over medium heat, heat oil. Add chicken and season with salt and pepper. Cook until golden and mostly cooked through, about 10 minutes. Remove from skillet and set aside. Add broccoli and carrots to skillet and season with more salt. Cook until tender, 5 minutes. Add garlic and ginger and cook until fragrant, 1 minute. Add soy sauce, honey, and sesame oil and stir until combined. Return chicken to skillet and toss to coat. In a large baking dish, add cooked rice, chicken-vegetable mixture, and toss until combined. Bake until casserole is heated through, 15 minutes. Garnish with green onions and sesame seeds before serving.',
      thumb:
        'https://www.themealdb.com/images/media/meals/wvpsxx1468256321.jpg',
      video: 'https://www.youtube.com/watch?v=ea4x4xv6Ytk',
      ingredients: [
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
        { name: 'boneless skinless chicken breasts', measure: '1 lb' },
        { name: 'salt', measure: '1/4 tsp' },
      ],
      isFavorite: false,
    };
    console.log(JSON.stringify(this.recipe));
    return this.recipe;
  }

  toggleFavorite() {
    if (this.recipe) {
      this.recipe.isFavorite = !this.recipe.isFavorite;
    }
  }
}

interface Recipe {
  id: number;
  title: string;
  instructions: string;
  thumb: string;
  video: string;
  ingredients: Ingredient[];
  isFavorite: boolean;
}

interface Ingredient {
  name: string;
  measure: string;
}
