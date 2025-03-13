/**
 * @file Defines the `ApiService` class, responsible for handling HTTP requests 
 * to fetch ingredient-related data from the backend API.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';

const INGREDIENT: string = '/api/Ingredient/';

/**
 * The `IngredientService` class provides methods for fetching ingredient data from the backend API.
 * It allows retrieval of all ingredient names as well as detailed information for specific ingredients.
 */
@Injectable({
  providedIn: 'root'
})
export class IngredientService {
  constructor(private http: HttpClient) { }

  /**
   * Fetches the list of all available ingredient names from the API.
   * 
   * @returns {Observable<string[]>} An observable containing an array of ingredient names.
   */
  getAllIngredients(): Observable<string[]> {
    return this.http.get<string[]>(INGREDIENT);
  }

  /**
   * Fetches detailed information about a specific ingredient from the API.
   * 
   * @param {string} itemName - The name of the ingredient to retrieve details for.
   * @returns {Observable<Ingredient>} An observable containing the ingredient details.
   */
  getSpecificIngredient(itemName: string): Observable<Ingredient> {
    return this.http.get<Ingredient>(INGREDIENT + itemName);
  }

  /**
   * Updates the database with missing ingredients from the external TACO API.
   * 
   * @param {Ingredient[]} ingredients - The list of ingredients to update.
   * @returns {Observable<any>} An observable with the API response.
   */
  updateDB(ingredients: Ingredient[]): Observable<any> {
    return this.http.post<{ message: string }>(INGREDIENT + 'update', ingredients, {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }


}

export interface Ingredient {
  name: string;
  kcal: number;
  kj: number;
  protein: number;
  carbohydrates: number;
  lipids: number;
  fibers: number;
  category: string;
}
