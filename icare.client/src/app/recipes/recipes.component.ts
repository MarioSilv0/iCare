import { Component } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';
import { RecipeService } from '../services/recipes.service';
import { UsersService } from '../services/users.service';
import { InventoryService } from '../services/inventory.service';
import { PermissionsService } from '../services/permissions.service';
import { Recipe, Permissions } from '../../models';
import { normalize } from '../utils/Normalize';
import { memoize } from '../utils/Memoization';
import { StorageUtil } from '../utils/StorageUtil';

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
  public inventoryPermission: boolean = false;

  public objectivesFilter: boolean = false;
  public preferencesFilter: boolean = false;
  public restrictionsFilter: boolean = false;
  public inventoryFilter: boolean = false;

  public filteredRecipes: Recipe[] = [];

  private restrictions: Set<string> | null = null;
  private preferences: Set<string> | null = null;
  private inventory: Map<string, number> | null = null;
  private normalizeMemoized = memoize(normalize);

  constructor(private recipesService: RecipeService, private userService: UsersService, private inventoryService: InventoryService, private permissionsService: PermissionsService) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(() => this.filterRecipes(''));
  }

  ngOnInit() {
    this.getPermissions();
    this.getRecipes();
  }

  toggleFavoriteRecipe(id: number) {
    let recipe = this.recipes[id]
    let old = recipe.isFavorite;

    this.recipesService.toggleFavorite(recipe.name).subscribe(
      (result) => recipe.isFavorite = !old,
      (error) => {
        console.error(error);
        recipe.isFavorite = old
      }
    )
  }

  getPermissions() {
    const permissions: Permissions | null = this.permissionsService.getPermissions();
    this.preferencesPermission = permissions?.preferences ?? false;
    this.restrictionPermission = permissions?.restrictions ?? false;
    this.inventoryPermission = permissions?.inventory ?? false;
  }

  getRecipes() {
    this.recipesService.getAllRecipes().subscribe(
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

    this.userService.getPreferences().subscribe(
      (result) => {
        this.preferences = new Set(result);
        if (this.preferencesFilter) this.filterRecipes('');
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getRestrictions() {
    if (this.restrictions) return;
    this.restrictions = new Set();

    this.userService.getRestrictions().subscribe(
      (result) => {
        this.restrictions = new Set(result);
        if (this.restrictionsFilter) this.filterRecipes('');
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getInventory() {
    if (this.inventory) return;
    this.inventory = new Map();

    this.inventoryService.getInventory().subscribe(
      (result) => {
        for (const item of result) {
          const quantity = item.unit === "kg" ? item.quantity * 1000 : item.quantity;
          this.inventory!.set(item.name, quantity);
        }

        if (this.inventoryFilter) this.filterRecipes('');
      },
      (error) => console.error(error)
    );
  }

  filterRecipes(newQuery: string) {
    
    const query = this.normalizeMemoized(newQuery);

    this.filteredRecipes = this.recipes.filter(r => this.normalizeMemoized(r.name).includes(query) || this.normalizeMemoized(r.category).includes(query) || this.normalizeMemoized(r.area).includes(query));

    if (this.preferencesFilter) this.filterPreferences();
    if (this.restrictionsFilter) this.filterRestrictions();
    if (this.inventoryFilter) this.filterInventory();
  }

  filterPreferences() {
    this.getPreferences();
    this.filteredRecipes = this.filteredRecipes.filter(r => this.preferences!.has(r.category));
  }

  filterRestrictions() {
    this.getRestrictions();
    this.filteredRecipes = this.filteredRecipes.filter(r => !this.restrictions!.has(r.category));
  }

  filterInventory() {
    this.getInventory();
    this.filteredRecipes = this.filteredRecipes.filter(r => {
      for (const ingredient of r.ingredients) {
        const quantity = this.inventory!.get(ingredient.name) || -1 ;
        if (!this.inventory!.has(ingredient.name) ||  quantity < ingredient.grams) return false;
      }

      return true;
    })
  }

  onSearchChange() {
    this.searchSubject.next();
  }
}
