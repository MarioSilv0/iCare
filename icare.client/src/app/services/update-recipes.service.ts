import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { env } from '../../environments/env';

@Injectable({
  providedIn: 'root'
})

export class UpdateRecipesService {
  private categoryListUrl = `${env.mealDbApiUrl}/categories.php`;
  private mealsByCategoryUrl = `${env.mealDbApiUrl}/filter.php?c=`;
  private mealByIdUrl = `${env.mealDbApiUrl}/lookup.php?i=`;

  public mealsList: any[] = [];
  constructor(private http: HttpClient) { }

  getCategories(): Observable<any> {
    return this.http.get<any>(this.categoryListUrl);
  }

  getMealsByCategory(category: string): Observable<any> {
    return this.http.get<any>(`${this.mealsByCategoryUrl}${category}`);
  }

  getMealById(id: string): Observable<any> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`);
  }

  async updateDatabase() {
    try {
      const categoriesResponse = await this.http.get<any>(this.categoryListUrl).toPromise();
      const categories = categoriesResponse.categories

      for (const category of categories) {
        await this.delay(500); // Pequeno atraso para evitar 429
        const mealsResponse = await this.http.get<any>(this.mealsByCategoryUrl + category.strCategory).toPromise();
        const meals = mealsResponse.meals;

        for (const meal of meals) {
          await this.delay(500); // Pequeno atraso para evitar 429
          const mealDetailsResponse = await this.http.get<any>(this.mealByIdUrl + meal.idMeal).toPromise();
          this.mealsList.push(mealDetailsResponse.meals[0]);
        }
      }
    } catch (error) {
      console.error('Erro ao atualizar a base de dados:', error);
    }
  }

  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
