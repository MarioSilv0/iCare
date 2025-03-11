/**
 * @file Defines the `ApiService` class, responsible for handling HTTP requests 
 * to fetch recipe-related data from the backend API.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * 
 * @date Last Modified: 2025-03-04
 */

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

const RECIPE: string = '/api/Recipe';


@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  constructor(private http: HttpClient) { }


  /**
   * Fetches all recipes from the API.
   * @returns An Observable containing an array of recipes.
   */
  getAllRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(RECIPE);
  }

  /**
   * Fetches a specific recipe by name.
   * @param recipeName The name of the recipe.
   * @returns An Observable containing the recipe details.
   */
  getSpecificRecipe(recipeName: string): Observable<Recipe> {
    return this.http.get<Recipe>(RECIPE + recipeName);
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

