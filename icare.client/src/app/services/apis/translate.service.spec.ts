import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TranslateService } from './translate.service';
import { env } from '../../../environments/env';
import { HttpErrorResponse } from '@angular/common/http';

/**
 * Testes unitários para o serviço TranslateService.
 * Verifica o comportamento de tradução de textos entre inglês e português.
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
   * Teste para traduzir texto do inglês para português.
   * Verifica se a tradução funciona corretamente e a requisição é feita para a URL correta.
   */
  it('should translate text from English to Portuguese', async () => {
    const mockResponse = { translatedText: 'Olá' };
    const textToTranslate = 'Hello';

    const translatedText = await service.translateENPT(textToTranslate);
    expect(translatedText).toBe('Olá');

    const req = httpMock.expectOne(env.translateUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      q: textToTranslate,
      source: 'en',
      target: 'pt'
    });

    req.flush(mockResponse);
  });

  /**
   * Teste para traduzir texto do português para inglês.
   * Verifica se a tradução funciona corretamente e a requisição é feita para a URL correta.
   */
  it('should translate text from Portuguese to English', async () => {
    const mockResponse = { translatedText: 'Hello' };
    const textToTranslate = 'Olá';

    const translatedText = await service.translatePTEN(textToTranslate);
    expect(translatedText).toBe('Hello');

    const req = httpMock.expectOne(env.translateUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      q: textToTranslate,
      source: 'pt',
      target: 'en'
    });

    req.flush(mockResponse);
  });

  /**
   * Teste para verificar o tratamento de erros da API de tradução.
   * Simula um erro na API e verifica se o serviço lida com o erro corretamente.
   */
  it('should handle translation API errors gracefully', async () => {
    const textToTranslate = 'Error test';

    try {
      await service.translateENPT(textToTranslate);
      fail('Expected an error, but got a response');
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        expect(error.status).toBe(500);
        expect(error.statusText).toBe('Internal Server Error');
      } else {
        fail('Expected an HttpErrorResponse, but got an unknown error');
      }
    }

    const req = httpMock.expectOne(env.translateUrl);
    req.flush('Internal Server Error', { status: 500, statusText: 'Internal Server Error' });
  });

});
