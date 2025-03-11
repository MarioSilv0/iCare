import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { env } from '../../../environments/env';
import { Observable, map, firstValueFrom } from 'rxjs';

/**
 * @file Defines the `TranslateService` class, responsible for handling text translations 
 * between different languages using an external translation API.
 *
 * @author Mário Silva  - 202000500
 * @date Last Modified: 2025-03-11
 */

@Injectable({
  providedIn: 'root'
})
export class TranslateService {
  private translateUrl = env.translateUrl;

  constructor(private http: HttpClient) { }

  /**
   * Translates a given text from English to Portuguese.
   * 
   * @param text The text to be translated.
   * @returns A promise resolving to the translated text in Portuguese.
   */
  async translateENPT(text: string): Promise<string> {
    return this.translateAPI(text, "en", "pt");
  }

  /**
   * Translates a given text from Portuguese to English.
   * 
   * @param text The text to be translated.
   * @returns A promise resolving to the translated text in English.
   */
  async translatePTEN(text: string): Promise<string> {
    return this.translateAPI(text, "pt", "en");
  }

  /**
   * Performs the actual translation request to the external API.
   * 
   * @param text The text to be translated.
   * @param from The source language code (e.g., "en" for English).
   * @param to The target language code (e.g., "pt" for Portuguese).
   * @returns A promise resolving to the translated text.
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
