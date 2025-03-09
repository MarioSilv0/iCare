import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { env } from '../../../environments/env';
import { Observable, map, firstValueFrom } from 'rxjs';

/**
 * Serviço para realizar traduções entre diferentes idiomas.
 * Utiliza a API de tradução para converter textos de um idioma para outro.
 */
@Injectable({
  providedIn: 'root'
})
export class TranslateService {
  private translateUrl = env.translateUrl;

  constructor(private http: HttpClient) { }

  /**
   * Traduz um texto do inglês para o português.
   * 
   * @param text Texto a ser traduzido.
   * @returns Uma promessa que resolve com o texto traduzido em português.
   */
  async translateENPT(text: string): Promise<string> {
    return this.translateAPI(text, "en", "pt");
  }

  /**
   * Traduz um texto do português para o inglês.
   * 
   * @param text Texto a ser traduzido.
   * @returns Uma promessa que resolve com o texto traduzido em inglês.
   */
  async translatePTEN(text: string): Promise<string> {
    return this.translateAPI(text, "pt", "en");
  }

  /**
   * Realiza a tradução de um texto usando a API de tradução.
   * 
   * @param text Texto a ser traduzido.
   * @param from Idioma de origem.
   * @param to Idioma de destino.
   * @returns Uma promessa que resolve com o texto traduzido.
   */
  private translateAPI(text: string, from: string, to: string): Promise<string> {
    const translation$ = this.http.post<any>(this.translateUrl, {
      q: text,
      source: from,
      target: to
    }).pipe(
      map(response => response.translatedText)
    );
    return firstValueFrom(translation$);
  }
}
