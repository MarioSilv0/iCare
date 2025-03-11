import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TacoApiService } from './taco-api.service';
import { env } from '../../../environments/env';
import { Ingredient } from '../ingredients.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

/**
 * @file Unit tests for TacoApiService.
 * Ensures proper API communication and data formatting for food ingredients.
 * 
 * @author Mário Silva - 202000500
 * @date Last Modified: 2025-03-11
 */

describe('TacoApiService', () => {
  let service: TacoApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TacoApiService, HttpClient]
    });

    service = TestBed.inject(TacoApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensures all HTTP requests are verified
  });

  /**
   * Test: Should call the correct API endpoint and return formatted ingredients.
   * Ensures API response is mapped correctly to the `Ingredient` model.
   */
  it('should call the correct API endpoint and return formatted ingredients', () => {
    const mockResponse = {
      data: {
        getAllFood: [
          {
            name: 'Apple',
            category: { name: 'Fruit' },
            nutrients: {
              kJ: 218,
              protein: 0.3,
              lipids: 0.2,
              kcal: 52,
              dietaryFiber: 2.4,
              carbohydrates: 13.8
            }
          },
          {
            name: 'Chicken Breast',
            category: { name: 'Meat' },
            nutrients: {
              kJ: 468,
              protein: 31,
              lipids: 3.6,
              kcal: 165,
              dietaryFiber: 0,
              carbohydrates: 0
            }
          }
        ]
      }
    };

    const expectedResult: Ingredient[] = [
      {
        name: 'Apple',
        category: 'Fruit',
        kj: 218,
        protein: 0.3,
        lipids: 0.2,
        kcal: 52,
        fibers: 2.4,
        carbohydrates: 13.8
      },
      {
        name: 'Chicken Breast',
        category: 'Meat',
        kj: 468,
        protein: 31,
        lipids: 3.6,
        kcal: 165,
        fibers: 0,
        carbohydrates: 0
      }
    ];

    service.getAllFood().subscribe(ingredients => {
      expect(ingredients).toEqual(expectedResult);
      expect(ingredients.length).toBe(2);
      expect(ingredients[0].name).toBe('Apple');
      expect(ingredients[1].category).toBe('Meat');
    });

    const req = httpMock.expectOne(env.tacoApiUrl);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  /**
   * Test: Should return an empty array when API response is empty.
   * Ensures the service correctly handles an empty response from the API.
   */
  it('should return an empty array when API response is empty', () => {
    const mockResponse = { data: { getAllFood: [] } };

    service.getAllFood().subscribe(ingredients => {
      expect(ingredients).toEqual([]);
      expect(ingredients.length).toBe(0);
    });

    const req = httpMock.expectOne(env.tacoApiUrl);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  /**
   * Test: Should handle an error response gracefully.
   * Simulates a server error and verifies proper error handling.
   */
  it('should handle an error response gracefully', () => {
    const errorMessage = 'Failed to load data';

    service.getAllFood().subscribe(
      () => fail('Expected an error, but got a response'),
      (error: HttpErrorResponse) => {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Server Error');
      }
    );

    const req = httpMock.expectOne(env.tacoApiUrl);
    expect(req.request.method).toBe('POST');
    req.flush(errorMessage, { status: 500, statusText: 'Server Error' });
  });

  /**
   * Test: Should handle an invalid API response.
   * Ensures the service does not crash when receiving an unexpected API format.
   */
  it('should handle invalid API response', () => {
    const invalidResponse = { data: { getAllFood: null } };

    service.getAllFood().subscribe(ingredients => {
      expect(ingredients).toEqual([]);
    });

    const req = httpMock.expectOne(env.tacoApiUrl);
    expect(req.request.method).toBe('POST');
    req.flush(invalidResponse);
  });
});
