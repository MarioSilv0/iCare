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
  getMealById(id: number): Observable<Recipe> {
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
   * @param meal - The meal data object.
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
   * @param id - The meal ID.
   * @returns An Observable with the translated meal data.
   */
  getMealByIdTranslated(id: number): Observable<Recipe | null> {
    return this.http.get<any>(`${this.mealByIdUrl}${id}`).pipe(
      switchMap(mealData => {
        if (!mealData?.meals?.length) {
          throw new Error('Receita não encontrada');
        }

        const meal = mealData.meals[0];

        // Faz o forkJoin para traduzir os campos
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
   * @param instructions - The instruction string to split.
   * @returns An array of instruction lines.
   */
  formatInstructions(instructions: string): string[] {
    return instructions.split(/\r\n|\r|\n/).filter(line => line.trim() !== '');
  }

  /**
   * Traduz um texto do inglês para português.
   */
  private translateText(text: string): Promise<string> {
    return this.translateService.translateENPT(text);
  }

  /**
   * Processa os ingredientes, incluindo tradução e conversão para gramas.
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
 * Atualiza o banco de dados de receitas, obtendo as categorias e refeições,
 * traduzindo os dados e armazenando as receitas processadas.
 */
  async updateRecipeDB() {
    try {
      const mealsList: Recipe[] = [];
      const categoriesList: string[] = [];

      const categoriesResponse = await this.getCategories().toPromise();
      const categories: [{ strCategory: string }] = categoriesResponse.categories ?? [];

      // Processa cada categoria
      for (let ic = 0; ic < categories.length; ic++) {
        const category = categories[ic];
        console.log(`categories: (${ic + 1}/${categories.length})`);
        const translatedCategory = await this.processCategory(category.strCategory);
        categoriesList.push(translatedCategory);

        // Obtém as refeições da categoria e processa
        const meals = await this.getMealsByCategoryAndTranslate(category.strCategory);
        mealsList.push(...meals);
      }

      if (mealsList.length > 0) {
        this.recipeService.updateRecipeDB(mealsList).subscribe(
          (response => {
            console.log('✅ Receita(s) atualizada(s) com sucesso:', response);
            return response; // Podes transformar a resposta se necessário
          }));
      }
    } catch (error) {
      console.error('Error updating the database:', error);
    }
  }

  /**
   * Traduz o nome de uma categoria.
   *
   * @param categoryName - O nome da categoria a ser traduzido.
   * @returns O nome traduzido da categoria.
   */
  private async processCategory(categoryName: string): Promise<string> {
    return await this.translateText(categoryName);
  }

  /**
   * Obtém as refeições de uma categoria e as traduz.
   *
   * @param categoryName - O nome da categoria para buscar as refeições.
   * @returns Uma lista de receitas traduzidas.
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
        this.recipeService.updateRecipeDB([mealDetails]).subscribe(
          (response => {
            console.log('response:', response);
          }));
      }
    }

    return translatedMeals;
  }


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
