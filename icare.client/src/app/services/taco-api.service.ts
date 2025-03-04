import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { env } from '../../environments/env';
import { Ingredient } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class TacoApiService {
  tacoApi = env.tacoApiUrl;

  constructor(private http: HttpClient) { }

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
        const foodList = response?.data?.getAllFood ?? []; // Se for null, substitui por []
        return foodList.map(food => ({
          name: food.name,
          category: food.category?.name ?? 'Unknown', // Evita erro se category for null
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

