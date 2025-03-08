import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, forkJoin, lastValueFrom, map, of, switchMap } from 'rxjs';
import { env } from '../../../environments/env';
import { IngredientRecipe, Recipe } from '../../../models/recipe';
import { TranslateService } from './translate.service';
import { MeasureConversionService } from '../measure-conversion.service';
import { RecipeService } from '../recipes.service';

@Injectable({
  providedIn: 'root'
})

/**
  * MealDbService - Service for fetching, processing, and translating meal data from TheMealDB API.
  *
  * Author: Mário
  *
  * This service provides methods to retrieve meal categories, meals by category, 
  * meal details, and translated meal information. It also processes ingredients, 
  * formats instructions, and updates a local meal database.
  */
export class MealDbService {
  private categoryListUrl = `${env.mealDbApiUrl}/categories.php`;
  private mealsByCategoryUrl = `${env.mealDbApiUrl}/filter.php?c=`;
  private mealByIdUrl = `${env.mealDbApiUrl}/lookup.php?i=`;

  public mealsList: Recipe[] = [];
  constructor(
    private http: HttpClient,
    private translateService: TranslateService,
    private measureConversionService: MeasureConversionService,
    private recipeService: RecipeService
  ) { }

  //getCategoriesTranslated(): Observable<any> {
  //  return this.http.get<any>(this.categoryListUrl);
  //}

  /**
   * Fetches meal categories from the MealDb API.
   *
   * @returns An Observable with the categories data.
   */
  getCategories(): Observable<any> {
    return this.http.get<any>(this.categoryListUrl);
  }

  /**
   * Fetches meals by category from the MealDb API.
   *
   * @param category - The category name.
   * @returns An Observable with the meals data.
   */
  getMealsByCategory(category: string): Observable<any> {
    return this.http.get<any>(`${this.mealsByCategoryUrl}${category}`);
  }

  /**
   * Fetches detailed meal data by ID and returns a Recipe object.
   *
   * @param id - The meal ID.
   * @returns An Observable with the Recipe.
   */
  getMealById(id: string): Observable<Recipe> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      map(response => {
        if (!response.meals || response.meals.length === 0) {
          throw new Error('Receita não encontrada');
        }

        const meal = response.meals[0];

        return {
          id: meal.idMeal,
          picture: meal.strMealThumb,
          name: meal.strMeal,
          category: meal.strCategory,
          area: meal.strArea,
          urlVideo: meal.strYoutube || '',
          instructions: this.formatInstructions(meal.strInstructions),
          ingredients: this.extractIngredients(meal),
          isFavorite: false
        };
      })
    );
  }

  /**
   * Extracts ingredients from the meal data.
   *
   * @param meal - The meal data object.
   * @returns An array of IngredientRecipe objects.
   */
  private extractIngredients(meal: any): IngredientRecipe[] {
    let ingredients: IngredientRecipe[] = [];

    for (let i = 1; i <= 20; i++) {
      const ingredient = meal[`strIngredient${i}`];
      const measure = meal[`strMeasure${i}`];

      if (ingredient && ingredient.trim() !== '') {
        ingredients.push({ name: ingredient, measure: measure, grams : 0 });
      }
    }

    return ingredients;
  }

  /**
   * Fetches a meal by ID, translates its details, and returns the translated meal data.
   *
   * @param id - The meal ID.
   * @returns An Observable with the translated meal data.
   */
  getMealByIdTranslated(id: string): Observable<Recipe> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      switchMap((mealData) => {
        if (!mealData?.meals?.length) {
          throw new Error('Receita não encontrada');
        }

        const meal = mealData.meals[0];
        const instructionsList = this.formatInstructions(meal.strInstructions);

        return forkJoin({
          name: this.translateText(meal.strMeal),
          category: this.translateText(meal.strCategory),
          area: this.translateText(meal.strArea),
          urlVideo: of(meal.strYoutube),
          picture: of(meal.strMealThumb),
          instructions: forkJoin(instructionsList.map(line => this.translateText(line))),
          ingredients: this.processIngredients(meal)
        }).pipe(
          map(({ name, category, area, urlVideo, picture, instructions, ingredients }) => ({
            id: Number(meal.idMeal),
            name,
            category,
            area,
            urlVideo,
            picture,
            instructions,
            ingredients,
            isFavorite: false
          }) as Recipe)
        );
      })
    );
  }

  /**
   * Splits the instruction string into an array of non-empty lines.
   *
   * This function splits the string on newline characters (\n, \r\n, or \r),
   * trims each line, and removes any empty lines.
   *
   * @param instructions - The instruction string to split.
   * @returns An array of instruction lines.
   */
  formatInstructions(instructions: string): string[] {
    return instructions
      .split(/\r\n|\r|\n/)
      .map(line => line.trim())
      .filter(line => line !== '');
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



  ///**
  //* Updates the database by fetching categories and meals,
  //* then processes and stores the translated recipes.
  //*/
  //async updateDatabase() {
  //  try {
  //    const categoriesResponse = await lastValueFrom(this.http.get<any>(this.categoryListUrl));
  //    const categories = categoriesResponse.categories;

  //    for (const category of categories) {
  //      await this.delay(500);
  //      const mealsResponse = await lastValueFrom(this.http.get<any>(this.mealsByCategoryUrl + category.strCategory));
  //      const meals = mealsResponse.meals;

  //      for (const meal of meals) {
  //        await this.delay(500);
  //        const mealDetailsResponse = await lastValueFrom(this.getMealByIdTranslated(meal.id));
  //        this.mealsList.push(mealDetailsResponse);
  //      }
  //    }

  //    if (this.mealsList.length > 0) {
  //      // Fazer um único request para enviar todas as receitas ao backend
  //      this.recipeService.addMultipleRecipes(this.mealsList).subscribe({
  //        next: () => console.log(`Successfully added ${this.mealsList.length} recipes.`),
  //        error: (err) => {
  //            return console.error(`Failed to add recipes: ${err}`);
  //        }
  //      });
  //    }
  //  } catch (error) {
  //    console.error('Error updating the database:', error);
  //  }
  //}


  /**
   * Returns a promise that resolves after a specified delay.
   *
   * @param ms - The delay in milliseconds.
   * @returns A promise that resolves after the delay.
   */
  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
