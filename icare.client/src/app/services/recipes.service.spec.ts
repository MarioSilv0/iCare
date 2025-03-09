import { TestBed } from '@angular/core/testing';
import { RecipeService } from './recipes.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Recipe } from '../../models/recipe';
import { HttpErrorResponse } from '@angular/common/http';

describe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

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

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RecipeService]
    });

    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should retrieve all recipes', () => {
    service.getAllRecipes().subscribe((recipes: Recipe[]) => {
      expect(recipes.length).toBe(1);
      expect(recipes[0].name).toBe('Bife com Batatas');
    });

    const req = httpMock.expectOne('/api/Recipe');
    expect(req.request.method).toBe('GET');
    req.flush([mockRecipe]); // Simula a resposta da API
  });

  it('should retrieve a specific recipe', () => {
    service.getSpecificRecipe('Bife com Batatas').subscribe((recipe: Recipe) => {
      expect(recipe).toBeTruthy();
      expect(recipe.name).toBe('Bife com Batatas');
    });

    const req = httpMock.expectOne('/api/RecipeBife com Batatas');
    expect(req.request.method).toBe('GET');
    req.flush(mockRecipe);
  });

  it('should update recipes successfully', () => {
    service.updateRecipeDB([mockRecipe]).subscribe((response: any) => {
      expect(response.message).toBe('Atualizado com sucesso');
    });

    const req = httpMock.expectOne('/api/Recipe/update');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual([mockRecipe]); // Verifica o corpo da requisição
    req.flush({ message: 'Atualizado com sucesso' });
  });

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
