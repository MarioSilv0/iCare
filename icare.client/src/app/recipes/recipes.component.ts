import { Component } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';
import { RecipeService, Recipe } from '../services/recipes.service';
import { StorageUtil } from '../utils/StorageUtil';
import { UsersService, Permissions } from '../services/users.service';

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

  public preferencesPermission: boolean = false;
  public restrictionPermission: boolean = false;

  public objectivesFilter: boolean = false;
  public preferencesFilter: boolean = false;
  public restrictionsFilter: boolean = false;

  public filteredRecipes: Recipe[] = [];

  private restrictions: Set<string> | null = null;
  private preferences: Set<string> | null = null;

  constructor(private api: RecipeService, private user: UsersService) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(() => this.filterRecipes());
  }

  ngOnInit() {
    this.getPermissions();
    this.getRecipes();
  }

  getPermissions() {
    const permissions: Permissions | null = this.user.getPermissions();
    this.preferencesPermission = permissions?.preferences ?? false;
    this.restrictionPermission = permissions?.restrictions ?? false;
  }

  toggleFavoriteRecipe(id: number) {
    this.recipes[id].isFavorite = !this.recipes[id].isFavorite;
  }

  getRecipes() {
    this.api.getAllRecipes().subscribe(
      (result) => {
        this.recipes = this.filteredRecipes = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getPreferences() {
    if (this.preferences) return;
    this.preferences = new Set();

    this.user.getPreferences().subscribe(
      (result) => {
        this.preferences = new Set(result);
        if (this.preferencesFilter) this.filterRecipes();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getRestrictions() {
    if (this.restrictions) return;
    this.restrictions = new Set();

    this.user.getRestrictions().subscribe(
      (result) => {
        this.restrictions = new Set(result);
        if (this.restrictionsFilter) this.filterRecipes();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  filterRecipes() {
    const query = this.searchTerm.toLowerCase().trim();
    this.filteredRecipes = this.recipes.filter(r => r.name.toLowerCase().includes(query));

    if (this.preferencesFilter) this.filterPreferences();
    if (this.restrictionsFilter) this.filterRestrictions();
  }

  filterPreferences() {
    this.getPreferences();
    this.filteredRecipes = this.filteredRecipes.filter(r => this.preferences!.has(r.category));
  }

  filterRestrictions() {
    this.getRestrictions();
    this.filteredRecipes = this.filteredRecipes.filter(r => !this.restrictions!.has(r.category));
  }

  onSearchChange() {
    this.searchSubject.next();
  }
}
