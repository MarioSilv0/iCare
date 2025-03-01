import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

const URL: string = 'https://localhost:7266/api/';
const INGREDIENT: string = 'Ingredient/';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) { }

  getAllItems(): Observable<string[]> {
    return this.http.get<string[]>(URL + INGREDIENT);
  }

  getSpecificItem(itemName: string): Observable<Ingredient> {
    return this.http.get<Ingredient>(URL + INGREDIENT + itemName);
  }
}

export interface Ingredient {
  name: string;
  kcal: number;
  kj: number;
  protein: number;
  carbohydrates: number;
  lipids: number;
  fibers: number;
  category: string;
}
