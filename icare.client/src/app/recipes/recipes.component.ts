import { Component } from '@angular/core';
import { RecipeService, Recipe } from '../services/recipes.service';
import { PROFILE } from '../services/users.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-recipes',
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css',
})
export class RecipesComponent {
  public filters: string[] = [
    'Inventário',
    'Meta Alimentar',
    'Restrições Alimentares',
    'Preferências Alimentares',
    'Favoritos',
  ];
  public recipes: Recipe[] = [];
  public searchTerm: string = '';
    recipe: any;
  constructor(private api: RecipeService, private http: HttpClient) { }

  ngOnInit() {
    this.getRecipes();
  }

  toggleFavoriteRecipe(id: number) {
    let recipe = this.recipes[id]
    let old = recipe.isFavorite;

    try {
      let url = `${PROFILE}/Recipe/${recipe.name}`
     
      this.http.put(url, {}).subscribe()

      recipe.isFavorite = !recipe.isFavorite;

      this.recipes[id].isFavorite = !this.recipes[id].isFavorite;
    } catch (e) {


      console.error(e);
      this.recipe.isFavorite = old
    }
    this.recipes[id].isFavorite = !this.recipes[id].isFavorite;
  }

  searchRecipes(searchTerm: string) {
    const filteredRecipes = this.recipes.forEach((r) => {
      r.name.includes(searchTerm) || r.description.includes(searchTerm);
    });
    console.log(filteredRecipes);
    // renderizar a nova lista
    // guardar a antiga lista de receitas
  }

  getRecipes() {
    this.api.getAllRecipes().subscribe(
      (result) => {
        this.recipes = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
