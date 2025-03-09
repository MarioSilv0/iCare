import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MealDbService } from './meal-db.service';
import { TranslateService } from './translate.service';
import { MeasureConversionService } from '../measure-conversion.service';
import { RecipeService } from '../recipes.service';
import { env } from '../../../environments/env';
import { of, throwError } from 'rxjs';
import { Recipe } from '../../../models';


describe('MealDbService', () => {
  let service: MealDbService;
  let httpMock: HttpTestingController;
  let translateServiceSpy: jasmine.SpyObj<TranslateService>;
  let measureConversionServiceSpy: jasmine.SpyObj<MeasureConversionService>;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

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

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch categories from API', () => {
    const mockCategories = { categories: [{ strCategory: 'Beef' }, { strCategory: 'Chicken' }] };
    service.getCategories().subscribe(categories => {
      expect(categories).toEqual(mockCategories);
    });
    const req = httpMock.expectOne(`${env.mealDbApiUrl}/categories.php`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategories);
  });

  it('should fetch meals by category', () => {
    const mockMeals = { meals: [{ strMeal: 'Steak', idMeal: 1234 }] };
    service.getMealsByCategory('Beef').subscribe(meals => {
      expect(meals).toEqual(mockMeals);
    });
    const req = httpMock.expectOne(`${env.mealDbApiUrl}/filter.php?c=Beef`);
    expect(req.request.method).toBe('GET');
    req.flush(mockMeals);
  });

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

  it('should return an empty array when no ingredients are found', () => {
    const result = (service as any).extractIngredients({});
    expect(result).toEqual([]);
  });

  it('should format instructions into an array', () => {
    const formatted = service.formatInstructions('Step 1\nStep 2\n');
    expect(formatted).toEqual(['Step 1', 'Step 2']);
  });

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
