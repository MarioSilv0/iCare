import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MealDbService } from './meal-db.service';
import { TranslateService } from './translate.service';
import { MeasureConversionService } from '../measure-conversion.service';
import { RecipeService } from '../recipes.service';
import { env } from '../../../environments/env';
import { of, throwError } from 'rxjs';
import { Recipe } from '../../../models';

/**
 * @file Unit tests for the `MealDbService`.
 * This test suite ensures the correct functionality of methods for fetching meal data,
 * processing meal categories, and translating the relevant details.
 * 
 * @author Mário
 * @date Last Modified: 2025-03-11
 */
describe('MealDbService', () => {
  let service: MealDbService;
  let httpMock: HttpTestingController;
  let translateServiceSpy: jasmine.SpyObj<TranslateService>;
  let measureConversionServiceSpy: jasmine.SpyObj<MeasureConversionService>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  /**
   * Configuração inicial dos testes. Define os espiões (spies) para os serviços injetados
   * e configura o módulo de testes com as dependências necessárias.
   */
  beforeEach(() => {
    const translateSpy = jasmine.createSpyObj('TranslateService', ['translateENPT']);
    const measureSpy = jasmine.createSpyObj('MeasureConversionService', ['addWeightToIngredient']);
    const recipeSpy = jasmine.createSpyObj('RecipeService', ['updateRecipeDB']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        MealDbService,
        { provide: TranslateService, useValue: translateSpy },
        { provide: MeasureConversionService, useValue: measureSpy },
        { provide: RecipeService, useValue: recipeSpy }
      ]
    });

    service = TestBed.inject(MealDbService);
    httpMock = TestBed.inject(HttpTestingController);
    translateServiceSpy = TestBed.inject(TranslateService) as jasmine.SpyObj<TranslateService>;
    measureConversionServiceSpy = TestBed.inject(MeasureConversionService) as jasmine.SpyObj<MeasureConversionService>;
    recipeServiceSpy = TestBed.inject(RecipeService) as jasmine.SpyObj<RecipeService>;
  });

  /**
   * Após cada teste, verifica se todas as requisições HTTP esperadas foram feitas.
   */
  afterEach(() => {
    httpMock.verify();
  });

  /**
   * Testa o método `getCategories` para garantir que ele recupera as categorias corretamente da API.
   * Verifica se a resposta da API é manipulada corretamente e se o serviço retorna as categorias esperadas.
   */
  it('should fetch categories from API', () => {
    const mockCategories = { categories: [{ strCategory: 'Beef' }, { strCategory: 'Chicken' }] };
    service.getCategories().subscribe(categories => {
      expect(categories).toEqual(mockCategories);
    });
    const req = httpMock.expectOne(`${env.mealDbApiUrl}/categories.php`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategories);
  });

  /**
   * Testa o método `getMealsByCategory` para garantir que ele recupera as refeições por categoria da API.
   * Verifica se o serviço chama a URL correta da API e processa a resposta adequadamente.
   */
  it('should fetch meals by category', () => {
    const mockMeals = { meals: [{ strMeal: 'Steak', idMeal: 1234 }] };
    service.getMealsByCategory('Beef').subscribe(meals => {
      expect(meals).toEqual(mockMeals);
    });
    const req = httpMock.expectOne(`${env.mealDbApiUrl}/filter.php?c=Beef`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMeals);
  });

  /**
   * Testa o método `getMealById` para garantir que ele recupera os detalhes de uma refeição pela ID da API.
   * Verifica se os detalhes da refeição são extraídos e retornados corretamente.
   */
  it('should fetch meal details by ID', () => {
    const mockMeal = { meals: [{ idMeal: 1234, strMeal: 'Steak', strInstructions: 'Cook it', strCategory: 'Beef' }] };
    service.getMealById(1234).subscribe(meal => {
      expect(meal.id).toBe(1234);
      expect(meal.name).toBe('Steak');
      expect(meal.instructions).toBe('Cook it');
    });
    const req = httpMock.expectOne(`${env.mealDbApiUrl}/lookup.php?i=1234`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMeal);
  });

  /**
   * Testa o método `extractIngredients` para garantir que ele retorna uma lista vazia
   * quando nenhum ingrediente é encontrado no objeto de refeição.
   */
  it('should return an empty array when no ingredients are found', () => {
    const result = (service as any).extractIngredients({});
    expect(result).toEqual([]);
  });

  /**
   * Testa o método `formatInstructions` para garantir que ele formate corretamente as instruções
   * de uma refeição em um array de linhas.
   */
  it('should format instructions into an array', () => {
    const formatted = service.formatInstructions('Step 1\nStep 2\n');
    expect(formatted).toEqual(['Step 1', 'Step 2']);
  });

  /**
   * Testa o método `processCategory` para garantir que ele traduza corretamente o nome da categoria
   * utilizando o serviço de tradução.
   */
  it('should process categories and translate them', async () => {
    const mockCategories = { categories: [{ strCategory: 'Beef' }, { strCategory: 'Chicken' }] };
    const mockTranslation = 'translated category';
    translateServiceSpy.translateENPT.and.returnValue(Promise.resolve(mockTranslation));

    // Simula a resposta da API
    httpMock.match(() => true)[0]?.flush(mockCategories);

    const translatedCategory = await service['processCategory']('Beef');
    expect(translatedCategory).toBe(mockTranslation);
  });
});
