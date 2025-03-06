import { Component } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';
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
  public searchSubject = new Subject<void>();

  public objectivesFilter: boolean = false;
  public preferencesFilter: boolean = false;
  public restrictionsFilter: boolean = false;

  public filteredRecipes: Recipe[] = [];

  private restrictions: string[] = [];
  private preferences: string[] = [];

  constructor(private api: RecipeService) {
    this.searchSubject
      .pipe(debounceTime(300))
      .subscribe(() => this.filterRecipes());
  }

  ngOnInit() {
    this.getPermissions();
    this.getRecipes();
  }

  getPermissions() {
    const permissions: Permissions | null = StorageUtil.getFromStorage('permissions');
    this.preferencesFilter = permissions?.preferences ?? false;
    this.restrictionsFilter = permissions?.restrictions ?? false;
  }

  toggleFavoriteRecipe(id: number) {
    this.recipes[id].isFavorite = !this.recipes[id].isFavorite;
  }

  getRecipes() {
    this.api.getAllRecipes().subscribe(
      (result) => {
        this.recipes = result;
        this.filteredRecipes = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  filterRecipes() {
    const query = this.searchTerm.toLowerCase().trim();
    this.filteredRecipes = this.recipes.filter(r => r.name.toLowerCase().includes(query));
  }

  onSearchChange() {
    this.searchSubject.next();
  }
}
