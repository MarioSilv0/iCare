

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { StorageUtil } from '../utils/StorageUtil';
import { Item } from '../../models';

export const INVENTORY: string = '/api/Inventory';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {

  constructor(private http: HttpClient) { }

  /**
   * Retrieves the inventory data from the API.
   * 
   * @returns {Observable<Item[]>} An observable containing an array of inventory items.
   */
  getInventory(): Observable<Item[]> {
    return this.http.get<Item[]>(INVENTORY);
  }

  /**
   * Updates the inventory data on the API.
   * 
   * @param {Item[]} items - The updated inventory list.
   * @returns {Observable<Item[]>} An observable containing the updated inventory items.
   */
  updateInventory(items: Item[]): Observable<Item[]> {
    return this.http.put<Item[]>(INVENTORY, items);
  }

  /**
   * Removes items from the inventory on the API.
   * 
   * @param {string[]} items - The names of the items to remove.
   * @returns {Observable<Item[]>} An observable containing the updated inventory after removal.
   */
  removeInventory(items: string[]): Observable<Item[]> {
    return this.http.delete<Item[]>(INVENTORY, { body: items });
  }
}
