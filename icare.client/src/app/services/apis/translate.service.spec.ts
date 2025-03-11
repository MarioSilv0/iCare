import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TranslateService } from './translate.service';
import { env } from '../../../environments/env';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * @file Unit tests for the `TranslateService`.
 * Ensures the correct behavior of text translation between English and Portuguese.
 * 
 * @author Mário Silva  - 202000500
 * @date Last Modified: 2025-03-11
 */

describe('TranslateService', () => {
  let service: TranslateService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TranslateService]
    });

    service = TestBed.inject(TranslateService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  /**
   * Tests translating text from English to Portuguese.
   * Verifies if the translation is performed correctly and the request is made to the correct URL.
   */
  it('should translate text from English to Portuguese', async () => {
    const mockResponse = { translatedText: 'Olá' };
    const textToTranslate = 'Hello';

    const translatePromise = service.translateENPT(textToTranslate);
    const req = httpMock.expectOne(env.translateUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      q: textToTranslate,
      source: 'en',
      target: 'pt'
    });
    req.flush(mockResponse);

    const translatedText = await translatePromise;
    expect(translatedText).toBe('Olá');
  });

  /**
   * Tests translating text from Portuguese to English.
   * Verifies if the translation is performed correctly and the request is made to the correct URL.
   */
  it('should translate text from Portuguese to English', async () => {
    const mockResponse = { translatedText: 'Hello' };
    const textToTranslate = 'Olá';

    const translatePromise = service.translatePTEN(textToTranslate);
    const req = httpMock.expectOne(env.translateUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      q: textToTranslate,
      source: 'pt',
      target: 'en'
    });
    req.flush(mockResponse);

    const translatedText = await translatePromise;
    expect(translatedText).toBe('Hello');
  });

  /**
   * Tests error handling when the translation API fails.
   * Simulates an API error and verifies if the service handles it correctly.
   */
  it('should handle translation API errors gracefully', async () => {
    const textToTranslate = 'Error test';
    const translatePromise = service.translateENPT(textToTranslate);

    const req = httpMock.expectOne(env.translateUrl);
    req.flush('Internal Server Error', { status: 500, statusText: 'Internal Server Error' });

    await expectAsync(translatePromise).toBeRejectedWith(jasmine.any(HttpErrorResponse));
  });
});
