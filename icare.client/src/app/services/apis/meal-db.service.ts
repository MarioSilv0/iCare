import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { env } from '../../../environments/env';
import { RecipeIngredient, Recipe } from '../../../models/recipe';
import { TranslateService } from './translate.service';
import { MeasureConversionService } from '../measure-conversion.service';
import { RecipeService } from '../recipes.service';

@Injectable({
  providedIn: 'root'
})

/**
 * Service for fetching, processing, and translating meal data from TheMealDB API.
 * 
 * This service provides methods to retrieve meal categories, meals by category, 
 * meal details, and translated meal information. It also processes ingredients, 
 * formats instructions, and updates a local meal database.
 * 
 * @author Mário Silva - 202000500
 * @date Last Modified: 2025-03-23
 */
export class MealDbService {
  private categoryListUrl = `${env.mealDbApiUrl}/categories.php`;
  private mealsByCategoryUrl = `${env.mealDbApiUrl}/filter.php?c=`;
  private mealByIdUrl = `${env.mealDbApiUrl}/lookup.php?i=`;

  constructor(
    private http: HttpClient,
    private translateService: TranslateService,
    private measureConversionService: MeasureConversionService,
    private recipeService: RecipeService
  ) { }

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
   * @param category The category name.
   * @returns An Observable with the meals data.
   */
  getMealsByCategory(category: string): Observable<any> {
    return this.http.get<any>(`${this.mealsByCategoryUrl}${category}`);
  }

  /**
   * Fetches detailed meal data by ID and returns a Recipe object.
   * 
   * @param id The meal ID.
   * @returns An Observable with the Recipe object.
   */
  getMealById(id: number): Observable<any> {
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
          instructions: meal.strInstructions,
          ingredients: this.extractIngredients(meal),
          isFavorite: false,
          calories: 0
        };
      })
    );
  }

  /**
   * Extracts ingredients from the meal data.
   * 
   * @param meal The meal data object.
   * @returns An array of RecipeIngredient objects.
   */
  private extractIngredients(meal: any): RecipeIngredient[] {
    let ingredients: RecipeIngredient[] = [];

    for (let i = 1; i <= 20; i++) {
      const ingredient = meal[`strIngredient${i}`];
      const measure = meal[`strMeasure${i}`];

      if (ingredient && ingredient.trim() !== '') {
        ingredients.push({ name: ingredient, measure: measure, grams: 0 });
      }
    }

    return ingredients;
  }

  /**
   * Fetches a meal by ID, translates its details, and returns the translated meal data.
   * 
   * @param id The meal ID.
   * @returns An Observable with the translated meal data.
   */
  getMealByIdTranslated(id: number): Observable<Recipe | null> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      switchMap(mealData => {
        if (!mealData?.meals?.length) {
          throw new Error('Receita não encontrada');
        }

        const meal = mealData.meals[0];

        // ForkJoin to translate fields
        return forkJoin({
          name: this.translateText(meal.strMeal),
          area: this.translateText(meal.strArea),
          category: this.translateText(meal.strCategory),
          urlVideo: of(meal.strYoutube),
          picture: of(meal.strMealThumb),
          instructions: this.translateText(meal.strInstructions),
          ingredients: this.processIngredients(meal)
        }).pipe(
          map(({ name, area, category, urlVideo, picture, instructions, ingredients }) => ({
            id: meal.idMeal,
            name,
            category,
            area,
            urlVideo,
            picture,
            instructions,
            ingredients,
            isFavorite: false,
            calories: 0
          }) as Recipe)
        );
      }),
      catchError(error => {
        console.warn(`Erro ao obter a refeição ID ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Splits the instruction string into an array of non-empty lines.
   * 
   * @param instructions The instruction string to split.
   * @returns An array of instruction lines.
   */
  formatInstructions(instructions: string): string[] {
    return instructions.split(/\r\n|\r|\n/).filter(line => line.trim() !== '');
  }

  /**
   * Translates text from English to Portuguese.
   * 
   * @param text The text to translate.
   * @returns A Promise that resolves with the translated text.
   */
  private translateText(text: string): Promise<string> {
    return this.translateService.translateENPT(text);
  }

  /**
   * Processes ingredients, including translation and conversion to grams.
   * 
   * @param meal The meal data object containing ingredients.
   * @returns An Observable with the processed ingredients.
   */
  private processIngredients(meal: any): Observable<{ name: string; measure: string; grams: number }[]> {
    const ingredientObservables: Promise<string>[] = [];
    const measureObservables: Promise<string>[] = [];
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

  /**
   * Updates the recipe database by fetching categories and meals, 
   * translating the data and storing the processed recipes.
   */
  async updateRecipeDB(updateProgress?: (message: string) => void) {
    try {

      const mealsList: Recipe[] = [];
      const categoriesList: string[] = [];

      const categoriesResponse = await this.getCategories().toPromise();
      const categories: [{ strCategory: string }] = categoriesResponse.categories ?? [];
      const totalCategories = categories.length;

      // Process each category
      for (let ic = 0; ic < totalCategories; ic++) {
        const category = categories[ic];

        console.log(`categories: (${ic + 1}/${categories.length})`);
        const progress = ((ic + 1) / totalCategories) * 100;
        if (updateProgress) {
          updateProgress(`Processando receitas da categoria (${ic + 1}/${totalCategories})`);
        }

        const translatedCategory = await this.processCategory(category.strCategory);
        categoriesList.push(translatedCategory);

        // Get meals from the category and process
        const meals = await this.getMealsByCategoryAndTranslate(category.strCategory);
        mealsList.push(...meals);

        //if (mealsList.length > 0) {
        //  this.recipeService.updateRecipeDB(mealsList).subscribe(() => {
        //    if (updateProgress) {
        //      updateProgress(`✅ Receita(s) da categoria "${translatedCategory}" atualizadas!`);
        //    }
        //  });
        //}
      }
      if (updateProgress) {
        updateProgress(`🎉 Atualização concluída!`);
      }
    } catch (error) {
      console.error('Error updating the database:', error);
    }
  }

  /**
   * Translates a category name.
   * 
   * @param categoryName The category name to translate.
   * @returns The translated category name.
   */
  private async processCategory(categoryName: string): Promise<string> {
    return await this.translateText(categoryName);
  }

  /**
   * Gets meals from a category and translates them.
   * 
   * @param categoryName The category name to get meals from.
   * @returns A list of translated Recipe objects.
   */
  private async getMealsByCategoryAndTranslate(categoryName: string): Promise<Recipe[]> {
    const mealsResponse = await this.getMealsByCategory(categoryName).toPromise();
    const meals = mealsResponse.meals ?? [];

    const translatedMeals = [];
    for (let im = 0; im < meals.length; im++) {
      const meal = meals[im];
      console.log(`meals: (${im + 1}/${meals.length})`);
      const mealDetails = await this.getMealByIdTranslated(meal.idMeal).toPromise();
      console.log(mealDetails);
      if (mealDetails) {
        translatedMeals.push(mealDetails);
        this.recipeService.updateRecipeDB([mealDetails]).subscribe((res) => {
          console.log(`(${im + 1}/${meals.length}) meal.`,res);
        });
      }
    }

    return translatedMeals;
  }

  /**
   * Returns a promise that resolves after a specified delay.
   * 
   * @param ms The delay in milliseconds.
   * @returns A promise that resolves after the delay.
   */
  private delay(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }
}
