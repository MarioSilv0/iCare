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
import { Observable } from 'rxjs';

const RECIPE: string = '/api/Recipe/';


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
}

interface Ingredient {
  name: string;
  quantity: number;
  unit: string;
}

export interface Recipe {
  picture: string;
  name: string;
  description: string;
  category: string;
  area: string;
  urlVideo: string;
  ingredients: Ingredient[];
  isFavorite: boolean;
  calories: number;
}
