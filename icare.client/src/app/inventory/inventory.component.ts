/**
 * @file Defines the `InventoryComponent` class, responsible for managing the user's inventory.
 * It provides functionalities for searching, selecting, modifying, and updating inventory items.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-05
 */

import { Component } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';
import { IngredientService, Ingredient } from '../services/ingredients.service';
import {
  addedItemNotification,
  editedItemNotification,
  NotificationService,
  removedItemNotification,
} from '../services/notifications.service';
import { Item, UsersService, Permissions } from '../services/users.service';
import { StorageUtil } from '../utils/StorageUtil';

declare var bootstrap: any;

/**
 * The `InventoryComponent` class is responsible for handling the user's inventory.
 * It allows users to add, remove, update, and search inventory items dynamically.
 */
@Component({
  selector: 'app-inventory',
  standalone: false,

  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css',
})
export class InventoryComponent {
  public notificationsPermission: boolean = true;
  public searchTerm: string = '';
  public searchSubject = new Subject<void>();

  public inventory: Map<string, { quantity: number; unit: string }> = new Map();
  public listOfItems: Set<string> = new Set();
  public filteredItems: string[] = [];

  public selectedItems: Set<string> = new Set();
  public selectedItemsInInventory: Set<string> = new Set();
  public expandedItems: Set<string> = new Set();
  public editedItems = new Set<string>();

  public itemDetails: Map<string, Ingredient> = new Map();

  public units: string[] = ['g', 'kg'];

  constructor(private service: UsersService, private api: IngredientService) {
    this.searchSubject
      .pipe(debounceTime(300))
      .subscribe(() => this.filterItems());
  }

  /**
   * Initializes the component by loading notification preferences and retrieving inventory data.
   */
  ngOnInit() {
    this.loadNotificationPreferences();
    this.getInventory();
  }

  /**
   * Loads user notification preferences from local storage.
   */
  private loadNotificationPreferences() {
    const permissions: Permissions | null = StorageUtil.getFromStorage('permissions');
    this.notificationsPermission = permissions?.notifications ?? false;
  }

  /**
   * @returns {Array} An array of inventory items. 
   */
  get inventoryArray() {
    return Array.from(this.inventory.entries());
  }

  /**
   * @returns {Array} An array of all available item names. 
   */
  get listOfItemsArray() {
    return Array.from(this.listOfItems);
  }

  /**
   * Toggles the selection state of an item in the inventory.
   * @param {string} item - The name of the item to toggle.
   */
  toggleSelection(item: string) {
    this.selectedItems.has(item) ? this.selectedItems.delete(item) : this.selectedItems.add(item);
  }

  /**
   * Toggles the selection state of an inventory item.
   * @param {string} item - The name of the item to toggle.
   */
  toggleInventorySelection(item: string) {
    this.selectedItemsInInventory.has(item) ? this.selectedItemsInInventory.delete(item) : this.selectedItemsInInventory.add(item);
  }

  /**
   * Toggles selection for all inventory items.
   */
  toggleAllInventorySelection() {
    this.selectedItemsInInventory.size === this.inventory.size
      ? this.selectedItemsInInventory.clear()
      : this.inventory.forEach((_, key) => this.selectedItemsInInventory.add(key));
  }

  /**
   * Toggles the expanded details view for a specific item.
   * @param {string} item - The name of the item.
   */
  toggleDetails(item: string) {
    this.expandedItems.has(item) ? this.expandedItems.delete(item) : this.expandedItems.add(item);
    if (!this.itemDetails.has(item)) {
      this.getItemDetails(item);
    }
  }

  /**
   * Handles search input changes and triggers filtering.
   */
  onSearchChange() {
    this.searchSubject.next();
  }

  /**
   * Filters available items based on the search term.
   */
  filterItems() {
    const query = this.searchTerm.toLowerCase().trim();
    this.filteredItems = Array.from(this.listOfItems).filter((n) => n.toLowerCase().includes(query));
  }

  /**
   * Retrieves the current inventory from the server.
   */
  getInventory() {
    this.service.getInventory().subscribe(
      (result) => {
        result.forEach((i) => this.inventory.set(i.name, { quantity: i.quantity, unit: i.unit }));
        this.getListItems();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Retrieves the list of all available items.
   */
  getListItems() {
    this.api.getAllIngredients().subscribe(
      (result) => {
        result.forEach((itemName) => { if (!this.inventory.has(itemName)) this.listOfItems.add(itemName); });
        this.filterItems();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Fetches details for a specific item.
   * @param {string} item - The name of the item.
   */
  getItemDetails(item: string): void {
    if (this.itemDetails.has(item)) return;

    this.api.getSpecificIngredient(item).subscribe(
      (result) => {
        let info = this.inventory.get(item) ?? { quantity: 1, unit: "g" };
        if (!info.unit) info.unit = "g";

        const total = info.unit === "g" ? 100 : 0.1;
        this.itemDetails.set(item, this.calculateIngredient(result, total, info.quantity));
      },
      (error) => {
        console.error(error);
        this.itemDetails.set(item, {
          name: `Não foi possível obter as informações do ingrediente: ${item}`,
          kcal: 0, kj: 0, protein: 0, carbohydrates: 0, lipids: 0, fibers: 0, category: 'none',
        });
      }
    );
  }

  /**
   * Updates the quantity of an inventory item.
   * @param {string} item - The name of the item.
   * @param {Event} event - The event from the input field.
   */
  updateQuantity(item: string, event: Event): void {
    const inventoryItem = this.inventory.get(item);
    if (!inventoryItem) return;

    const input = event.target as HTMLInputElement;
    let newValue = Math.max(0, Math.round(+input.value * 100) / 100);
    input.value = newValue.toFixed(2);
    if (newValue === inventoryItem.quantity) return;

    const previousQuantity = inventoryItem.quantity;
    if (!previousQuantity) this.itemDetails.delete(item);

    inventoryItem.quantity = newValue;
    this.editedItems.add(item);

    if (this.itemDetails.has(item)) this.itemDetails.set(item, this.calculateIngredient(this.itemDetails.get(item)!, previousQuantity, newValue));
    else this.getItemDetails(item);
  }

  /**
   * Retrieves the unit of measurement for a given inventory item.
   * If the item is not found in the inventory, it returns an empty string.
   *
   * @param {string} item - The name of the item.
   * @returns {string} The unit of measurement for the specified item.
   */
  getUnit(item: string): string {
    return this.inventory.get(item)?.unit || '';
  }

  /**
   * Calculates the progress or quantity representation of an inventory item.
   * If the item is measured in grams (`g`), it returns the quantity as is.
   * If the item is measured in kilograms (`kg`), it converts it to grams by multiplying by 1000.
   * If the item is not found in the inventory, it defaults to 100 (assuming full progress).
   *
   * @param {string} item - The name of the inventory item.
   * @returns {number} The adjusted quantity based on the unit of measurement.
   */
  getProgress(item: string): number {
    const i = this.inventory.get(item);
    if (!i) return 100;

    return (i.unit === "g") ? i.quantity : i.quantity * 1000;
  }

  /**
   * Updates the unit of measurement for an inventory item.
   * @param {string} item - The name of the item.
   * @param {Event} event - The event from the input field.
   */
  updateUnit(item: string, event: Event) {
    const inventoryItem = this.inventory.get(item);
    if (!inventoryItem) return;

    const previousUnit = inventoryItem.unit;
    inventoryItem.unit = (event.target as HTMLSelectElement).value;
    this.editedItems.add(item);

    if (this.itemDetails.has(item)) {
      const ingredient = this.itemDetails.get(item)!;
      this.convertIngredientUnits(ingredient, previousUnit, inventoryItem.unit);
    }
  }

  /**
   * Adds selected items to the inventory.
   */
  addItemsToInventory() {
    const addedItems: Item[] = Array.from(this.selectedItems).map((n) => {
      return { name: n, quantity: 1, unit: '' };
    });

    this.service.updateInventory(addedItems).subscribe(
      (result) => {
        const res: Set<string> = new Set(result.map((i) => i.name));

        const successfullyAdded: string[] = [...this.selectedItems].filter((name) => res.has(name));
        if (successfullyAdded.length !== this.selectedItems.size) console.error('Failed to add items!');

        for (let name of successfullyAdded) {
          this.inventory.set(name, { quantity: 1, unit: '' });
          this.listOfItems.delete(name);
        }

        this.selectedItems.clear();
        this.filterItems();
        NotificationService.showNotification(this.notificationsPermission, addedItemNotification);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Checks if there are any inventory items with a quantity of zero.
   * If found, prompts the user to confirm deletion; otherwise, updates the inventory.
   */
  checkForEmptyItems() {
    const hasEmptyItems = Array.from(this.inventory.values()).some((item) => item.quantity === 0);
    hasEmptyItems ? this.openModal('deleteZeroQuantityModal') : this.updateItemsInInventory();
  }

  /**
   * Updates the inventory with modified item quantities and units.
   * If `removeZeroQuantity` is true, removes items with zero quantity after updating.
   *
   * @param {boolean} [removeZeroQuantity=false] - Whether to remove items with zero quantity after updating.
   */
  updateItemsInInventory(removeZeroQuantity: boolean = false) {
    const updatedItems: Item[] = Array.from(this.editedItems).map((n) => {
      return {
        name: n,
        quantity: this.inventory.get(n)?.quantity ?? 0,
        unit: this.inventory.get(n)?.unit ?? '',
      };
    });
    if (updatedItems.length === 0 && !removeZeroQuantity) return;

    this.service.updateInventory(updatedItems).subscribe(
      (result) => {
        const resultMap = new Map(
          result.map((item) => [
            item.name,
            { quantity: item.quantity, unit: item.unit },
          ])
        );

        for (let item of this.editedItems) {
          const updatedValue = resultMap.get(item) ?? { quantity: 0, unit: '' };
          (!updatedValue) ? this.inventory.delete(item) : this.inventory.set(item, { ...updatedValue });
        }

        this.editedItems.clear();
        
        if (removeZeroQuantity) {
          const itemsToDelete = Array.from(this.inventory.keys()).filter((name) => this.inventory.get(name)?.quantity === 0);
          this.removeItemFromInventory(itemsToDelete);
        }

        NotificationService.showNotification(this.notificationsPermission, editedItemNotification);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Removes selected items from the inventory.
   * @param {string[]} [itemsToDelete] - Optional list of items to delete.
   */
  removeItemFromInventory(itemsToDelete: string[] | null | undefined) {
    const itemsToRemove: string[] =
      itemsToDelete ?? Array.from(this.selectedItemsInInventory);
    if (itemsToRemove.length === 0) return;

    this.service.removeInventory(itemsToRemove).subscribe(
      (result) => {
        const res = new Set(result.map((i) => i.name));

        const deletedItems = itemsToRemove.filter((name) => !res.has(name));
        if (deletedItems.length !== itemsToRemove.length) console.error('Failed to delete items!');

        for (let name of deletedItems) {
          this.listOfItems.add(name);
          this.inventory.delete(name);
          this.itemDetails.delete(name);
          this.expandedItems.delete(name);
        }

        if (!itemsToDelete) this.selectedItemsInInventory.clear();
        this.filterItems();
        NotificationService.showNotification(this.notificationsPermission, removedItemNotification);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
   * Opens a Bootstrap modal by its ID.
   * @param {string} id - The ID of the modal to be opened.
   */
  openModal(id: string) {
    const modalElement = document.getElementById(id);
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  /**
 * Converts the nutritional values of an ingredient between grams (`g`) and kilograms (`kg`).
 * Adjusts the kcal, kj, protein, carbohydrates, lipids, and fibers accordingly.
 *
 * @param {Ingredient} ingredient - The ingredient whose values need to be converted.
 * @param {string} fromUnit - The original unit of measurement (`g` or `kg`).
 * @param {string} toUnit - The target unit of measurement (`g` or `kg`).
 */
  private convertIngredientUnits(ingredient: Ingredient, fromUnit: string, toUnit: string) {
    if (fromUnit === toUnit) return;

    if (fromUnit === 'g') {
      ingredient.kcal *= 1000;
      ingredient.kj *= 1000;
      ingredient.protein *= 1000;
      ingredient.carbohydrates *= 1000;
      ingredient.lipids *= 1000;
      ingredient.fibers *= 1000;
    } else {
      ingredient.kcal /= 1000;
      ingredient.kj /= 1000;
      ingredient.protein /= 1000;
      ingredient.carbohydrates /= 1000;
      ingredient.lipids /= 1000;
      ingredient.fibers /= 1000;
    }
  }

  /**
 * Recalculates the nutritional values of an ingredient based on the new quantity.
 * The values are proportionally adjusted according to the total reference amount.
 *
 * @param {Ingredient} result - The ingredient whose values need to be adjusted.
 * @param {number} total - The total reference amount (e.g., 100g or 1kg).
 * @param {number} newQuantity - The new quantity of the ingredient.
 * @returns {Ingredient} A new ingredient object with recalculated nutritional values.
 */
  private calculateIngredient(result: Ingredient, total: number, newQuantity: number): Ingredient {
    if (total === 0) return { ...result, kcal: 0, kj: 0, protein: 0, carbohydrates: 0, lipids: 0, fibers: 0 };

    const decimalPlaces = 1000000;
    const newIngredient = { ...result };

    newIngredient.kcal = Math.round(((result.kcal * newQuantity) / total) * decimalPlaces) / decimalPlaces;
    newIngredient.kj = Math.round(((result.kj * newQuantity) / total) * decimalPlaces) / decimalPlaces;
    newIngredient.protein = Math.round(((result.protein * newQuantity) / total) * decimalPlaces) / decimalPlaces;
    newIngredient.carbohydrates = Math.round(((result.carbohydrates * newQuantity) / total) * decimalPlaces) / decimalPlaces;
    newIngredient.lipids = Math.round(((result.lipids * newQuantity) / total) * decimalPlaces) / decimalPlaces;
    newIngredient.fibers = Math.round(((result.fibers * newQuantity) / total) * decimalPlaces) / decimalPlaces;

    return newIngredient;
  }
}
