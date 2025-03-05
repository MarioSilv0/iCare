import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { env } from '../../../environments/env';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TranslateService {
  private translateUrl = env.translateUrl;

  constructor(private http: HttpClient) { }

  translateENPT(text: string): Observable<string> {
    return this.translateAPI(text, "en", "pt");
  }

  translatePTEN(text: string): Observable<string> {
    return this.translateAPI(text, "pt", "en");
  }

  private translateAPI(text: string, from: string, to: string): Observable<string> {
    return this.http.post<any>(this.translateUrl, {
      q: text,
      source: from,
      target: to
    }).pipe(
      map(response => response.translatedText)
    );
  }


}
