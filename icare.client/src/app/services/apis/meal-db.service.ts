import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, forkJoin, map, switchMap } from 'rxjs';
import { env } from '../../../environments/env';
import { IngredientRecipe, Recipe } from '../../../models/recipe';
import { TranslateService } from './translate.service';
import { MeasureConversionService } from '../measure-conversion.service';

@Injectable({
  providedIn: 'root'
})

export class MealDbService {
  private categoryListUrl = `${env.mealDbApiUrl}/categories.php`;
  private mealsByCategoryUrl = `${env.mealDbApiUrl}/filter.php?c=`;
  private mealByIdUrl = `${env.mealDbApiUrl}/lookup.php?i=`;

  public mealsList: Recipe[] = [];
  constructor(private http: HttpClient, private translateService: TranslateService, private measureConversionService: MeasureConversionService) { }

  getCategories(): Observable<any> {
    return this.http.get<any>(this.categoryListUrl);
  }

  getMealsByCategory(category: string): Observable<any> {
    return this.http.get<any>(`${this.mealsByCategoryUrl}${category}`);
  }

  getMealById(id: string): Observable<Recipe> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      map(response => {
        if (!response.meals || response.meals.length === 0) {
          throw new Error('Receita não encontrada');
        }

        const meal = response.meals[0];

        return {
          id: meal.idMeal,
          title: meal.strMeal,
          instructions: meal.strInstructions,
          thumb: meal.strMealThumb,
          video: meal.strYoutube || '',
          ingredients: this.extractIngredients(meal),
          isFavorite: false
        };
      })
    );
  }

  private extractIngredients(meal: any): IngredientRecipe[] {
    let ingredients: IngredientRecipe[] = [];

    for (let i = 1; i <= 20; i++) {
      const ingredient = meal[`strIngredient${i}`];
      const measure = meal[`strMeasure${i}`];

      if (ingredient && ingredient.trim() !== '') {
        ingredients.push({ name: ingredient, measure: measure, grams : "" || '' });
      }
    }

    return ingredients;
  }



  getMealByIdTranslated(id: string): Observable<any> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      switchMap((mealData) => {
        if (!mealData?.meals?.length) {
          throw new Error('Receita não encontrada');
        }

        const meal = mealData.meals[0];

        return forkJoin({
          title: this.translateText(meal.strMeal),
          instructions: this.translateText(meal.strInstructions),
          ingredients: this.processIngredients(meal)
        }).pipe(
          map(({ title, instructions, ingredients }) => ({
            id: Number(meal.idMeal),
            title,
            instructions,
            thumb: meal.strMealThumb,
            video: meal.strYoutube,
            isFavorite: false,
            ingredients
          }))
        );
      })
    );
  }

  /**
   * Traduz um texto do inglês para português.
   */
  private translateText(text: string): Observable<string> {
    return this.translateService.translateENPT(text) as Observable<string>;
  }

  /**
   * Processa os ingredientes, incluindo tradução e conversão para gramas.
   */
  private processIngredients(meal: any): Observable<{ name: string; measure: string; grams: number }[]> {
    const ingredientObservables: Observable<string>[] = [];
    const measureObservables: Observable<string>[] = [];
    const ingredients: { name: string; measure: string }[] = [];

    for (let i = 1; i <= 20; i++) {
      const name = meal[`strIngredient${i}`];
      const measure = meal[`strMeasure${i}`];

      if (name?.trim()) {
        ingredients.push({ name, measure });
        ingredientObservables.push(this.translateText(name));
        measureObservables.push(this.translateText(measure));
      }
    }

    return forkJoin({
      ingredients: forkJoin(ingredientObservables),
      measures: forkJoin(measureObservables)
    }).pipe(
      map(({ ingredients, measures }) =>
        ingredients.map((name, index) => {
          const measure = measures[index] || '';
          return this.measureConversionService.addWeightToIngredient({ name, measure });
        })
      )
    );
  }



  //formatInstructions(instructions: string): string[] {
  //return instructions.split(/\r\n|\r|\n/).filter(line => line.trim() !== '');
  //} 


  async updateDatabase() {
    try {
      const categoriesResponse = await this.http.get<any>(this.categoryListUrl).toPromise();
      const categories = categoriesResponse.categories

      for (const category of categories) {
        await this.delay(500);
        const mealsResponse = await this.http.get<any>(this.mealsByCategoryUrl + category.strCategory).toPromise();
        const meals = mealsResponse.meals;

        for (const meal of meals) {
          await this.delay(500);
          const mealDetailsResponse = await this.getMealByIdTranslated(meal.id);
          //this.mealsList.push(mealDetailsResponse);
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
