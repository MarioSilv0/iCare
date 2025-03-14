import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { env } from '../../../environments/env';
import { Ingredient } from '../../../models';

/**
 * @file Service for retrieving food ingredient data from the TACO API.
 * Fetches food items along with their nutritional values.
 * 
 * @author Mário Silva  - 202000500
 * @date Last Modified: 2025-03-11
 */

@Injectable({
  providedIn: 'root'
})
export class TacoApiService {
  private tacoApi = env.tacoApiUrl;

  constructor(private http: HttpClient) { }

  /**
   * Fetches all available food ingredients from the TACO API.
   * 
   * @returns An observable containing an array of `Ingredient` objects.
   */
  getAllFood(): Observable < Ingredient[] > {
    const graphqlQuery = {
      query: `
        query {
          getAllFood {
              name
              category {
                name
              }
              nutrients {
                kJ
                protein
                lipids
                kcal
                dietaryFiber
                carbohydrates
              }
          }
        }
      `
    };
    return this.http.post<{ data: { getAllFood: any[] } }>(this.tacoApi, graphqlQuery).pipe(
      map(response => {
        const foodList = response?.data?.getAllFood ?? [];
        return foodList.map(food => ({
          name: food.name,
          category: food.category?.name ?? 'Unknown',
          kj: food.nutrients?.kJ ?? 0,
          protein: food.nutrients?.protein ?? 0,
          lipids: food.nutrients?.lipids ?? 0,
          kcal: food.nutrients?.kcal ?? 0,
          fibers: food.nutrients?.dietaryFiber ?? 0,
          carbohydrates: food.nutrients?.carbohydrates ?? 0
        }) as Ingredient);
      })
    );

   }


}

