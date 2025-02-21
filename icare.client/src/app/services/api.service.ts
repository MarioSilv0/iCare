import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

const URL: string = 'https://localhost:7266/api/';
const INVENTORY: string = 'Inventory/';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) { }

  getAllItems(): Observable<Item[]> {
    return this.http.get<Item[]>(URL + INVENTORY);
  }

  getSpecificItem(itemName: string): Observable<Item | Item[]> {
    return this.http.get<Item[]>(URL + INVENTORY);
  }
}

export interface Item {
  name: string;
  quantity: number;
}
