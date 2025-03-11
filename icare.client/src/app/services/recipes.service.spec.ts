/**
 * @file Defines the unit tests for the `RecipeService` class, ensuring correct functionality 
 * of methods responsible for interacting with the recipe-related API.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * 
 * @date Last Modified: 2025-03-04
 */

import { TestBed } from '@angular/core/testing';
import { RecipeService } from './recipes.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Recipe } from '../../models/recipe';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Unit tests for the RecipeService class.
 * This file contains tests to verify the correctness of the service's behavior when interacting with the backend API.
 */
describe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  // Mock recipe data for testing
  const mockRecipe: Recipe = {
    id: 1,
    picture: 'image.jpg',
    name: 'Bife com Batatas',
    category: 'Carne',
    area: 'Brasileira',
    instructions: 'Fritar o bife e servir com batatas.',
    ingredients: [
      { name: 'Bife', measure: '200g', grams: 200 },
      { name: 'Batata', measure: '3 unidades', grams: 300 }
    ],
    urlVideo: 'https://video.com/bife',
    isFavorite: false,
    calories: 500
  };

  /**
   * Initializes the testing environment by configuring the TestBed with necessary imports 
   * and providers for testing the RecipeService.
   */
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RecipeService]
    });

    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  /**
   * Cleans up after each test, ensuring there are no pending HTTP requests.
   */
  afterEach(() => {
    httpMock.verify();
  });

  /**
   * Test to verify that the getAllRecipes method correctly retrieves all recipes from the backend API.
   * Ensures that the response contains the correct number of recipes and their data.
   */
  it('should retrieve all recipes', () => {
    service.getAllRecipes().subscribe((recipes: Recipe[]) => {
      expect(recipes.length).toBe(1);
      expect(recipes[0].name).toBe('Bife com Batatas');
    });

    const req = httpMock.expectOne('/api/Recipe');
    expect(req.request.method).toBe('GET');
    req.flush([mockRecipe]);
  });

  /**
   * Test to verify that the getSpecificRecipe method correctly retrieves a specific recipe by name.
   * Ensures that the returned recipe matches the requested name.
   * 
   * @param recipeName The name of the recipe to fetch.
   */
  it('should retrieve a specific recipe', () => {
    service.getSpecificRecipe('Bife com Batatas').subscribe((recipe: Recipe) => {
      expect(recipe).toBeTruthy();
      expect(recipe.name).toBe('Bife com Batatas');
    });

    const req = httpMock.expectOne('/api/RecipeBife com Batatas');
    expect(req.request.method).toBe('GET');
    req.flush(mockRecipe);
  });

  /**
   * Test to verify that the updateRecipeDB method correctly updates recipes in the backend.
   * Ensures that the correct HTTP method (PUT) is used and the correct request body is sent.
   */
  it('should update recipes successfully', () => {
    service.updateRecipeDB([mockRecipe]).subscribe((response: any) => {
      expect(response.message).toBe('Atualizado com sucesso');
    });

    const req = httpMock.expectOne('/api/Recipe/update');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual([mockRecipe]);
    req.flush({ message: 'Atualizado com sucesso' });
  });

  /**
   * Test to verify that the updateRecipeDB method correctly handles errors during the update process.
   * Ensures that an error is thrown with the expected message when the backend returns an error response.
   * 
   * @param error The error object returned by the HTTP request.
   *
   * @author Mário Silva  - 202000500
   * @date Last Modified: 2025-03-11
   */
  it('should handle error on updateRecipeDB', () => {
    service.updateRecipeDB([mockRecipe]).subscribe({
      error: (error: HttpErrorResponse) => {
        expect(error).toBeInstanceOf(Error);
        expect(error.message).toBe('Erro ao atualizar receitas. Tente novamente mais tarde.');
      }
    });

    const req = httpMock.expectOne('/api/Recipe/update');
    expect(req.request.method).toBe('PUT');
    req.flush('Erro interno', { status: 500, statusText: 'Internal Server Error' });
  });

});
