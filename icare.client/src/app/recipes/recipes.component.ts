import { Component } from '@angular/core';
import { RecipeService, Recipe } from '../services/recipes.service';
import { StorageUtil } from '../utils/StorageUtil';
import { Permissions } from '../services/users.service';

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

  public preferencesFilter: boolean = false;
  public restrictionsFilter: boolean = false;

  constructor(private api: RecipeService) { }

  ngOnInit() {
    this.getPermissions();
    this.getRecipes();
  }

  getPermissions() {
    const permissions: Permissions | null = StorageUtil.getFromStorage('permissions');
    this.preferencesFilter = permissions?.preferences ?? false;
    this.restrictionsFilter = permissions?.restrictions ?? false;

    console.log(this.preferencesFilter, this.restrictionsFilter)
  }

  toggleFavoriteRecipe(id: number) {
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
