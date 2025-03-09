/**
 * @file Defines the `ApiService` class, responsible for handling HTTP requests 
 * to fetch recipe-related data from the backend API.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-04
 */

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Recipe } from '../../models/recipe';
import { catchError, map } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';

const RECIPE: string = '/api/Recipe';


@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  constructor(private http: HttpClient) { }


  getAllRecipes(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(RECIPE);
  }


  getSpecificRecipe(recipeName: string): Observable<Recipe> {
    return this.http.get<Recipe>(RECIPE + recipeName);
  }

  /**
   * Sends multiple recipes to the backend in a single request.
   * @param recipes The array of recipes to be added.
   * @returns An Observable with the result of the HTTP request.
   */
  updateRecipeDB(recipes: Recipe[]): Observable<any> {
    return this.http.put<any>(`${RECIPE}/update`, recipes);
  }



}

