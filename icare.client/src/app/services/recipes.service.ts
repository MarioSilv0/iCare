/**
 * @file Defines the `RecipeService` class, responsible for handling HTTP requests 
 * to fetch recipe-related data from the backend API.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * 
 * @date Last Modified: 2025-03-05
 */

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';
import { PROFILE } from './users.service';

const RECIPE: string = '/api/Recipe';

/**
 * The `RecipeService` class provides methods for fetching recipe data from the backend API.
 */
@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  constructor(private http: HttpClient) { }

  /**
   * Retrieves a list of all available recipes from the backend.
   * 
   * @returns {Observable<Recipe[]>} An observable containing an array of recipes.
   */
  getAllRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(RECIPE);
  }

  /**
   * Fetches the details of a specific recipe based on its name.
   * 
   * @param {string} recipeName - The name of the recipe to retrieve.
   * @returns {Observable<Recipe>} An observable containing the recipe details.
   */
  getSpecificRecipe(recipeName: string): Observable<Recipe> {
    return this.http.get<Recipe>(`${RECIPE}/${recipeName}`);
  }

  toggleFavorite(recipeName: string): Observable<string> {
    return this.http.put<string> (`${PROFILE}/Recipe/${recipeName}`, {});
  }

  /**
   * Sends multiple recipes to the backend in a single request.
   * @param recipes The array of recipes to be added.
   * @returns An Observable with the result of the HTTP request.
   *
   * @author Mário Silva  - 202000500
   * @date Last Modified: 2025-03-11
   */
  updateRecipeDB(recipes: Recipe[]): Observable<any> {
    return this.http.put<any>(`${RECIPE}/update`, recipes).pipe(
      catchError(() => {
        return throwError(() => new Error('Erro ao atualizar receitas. Tente novamente mais tarde.'));
      })
    );
  }

}

