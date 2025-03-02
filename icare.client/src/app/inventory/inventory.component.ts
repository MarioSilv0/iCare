/**
 * @file Defines the `InventoryComponent` class, responsible for managing the user's inventory.
 * It provides functionalities for searching, selecting, modifying, and updating inventory items.
 * 
 * @author João Morais  - 202001541
 * @author Luís Martins - 202100239
 * @author Mário Silva  - 202000500
 * 
 * @date Last Modified: 2025-03-01
 */

import { Component } from '@angular/core';
import { UsersService, Item } from '../services/users.service';
import { ApiService, Ingredient } from '../services/api.service';
import { NotificationService, addedItemNotification, editedItemNotification, removedItemNotification } from '../services/notifications.service';
import { debounceTime, Subject } from "rxjs";

declare var bootstrap: any;

@Component({
  selector: 'app-inventory',
  standalone: false,

  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css'
})

/**
  * The `InventoryComponent` class manages the user's inventory,
  * allowing the user to add, remove, and update items while handling notifications.
  */
export class InventoryComponent {
  private notificationsPermission: boolean = true;
  public searchTerm: string = "";
  public searchSubject = new Subject<string>();

  public inventory: Map<string, { quantity: number, unit: string }> = new Map();
  public listOfItems: Set<string> = new Set();
  public filteredItems: string[] = [];

  public selectedItems: Set<string> = new Set();
  public selectedItemsInInventory: Set<string> = new Set();
  public expandedItems: Set<string> = new Set();
  public editedItems = new Set<string>();

  public itemDetails: Map<string, Ingredient> = new Map();

  constructor(private service: UsersService, private api: ApiService) {
    this.searchSubject.pipe(debounceTime(300)).subscribe(() => this.filterItems());
  }

  /**
   * Initializes the inventory component, retrieves user notification settings,
   * and loads the current inventory.
   */
  ngOnInit() {
    try {
      const storage = localStorage.getItem('user');
      if (storage) {
        this.notificationsPermission = JSON.parse(storage).notifications || true;
      }
    } catch (e) {
      console.error('Failed to get user data in localStorage:', e);
    }

    this.getInventory();
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
   * Toggles the selection state of an item in the list.
   * @param {string} item - The name of the item to toggle.
   */
  toggleSelection(item: string) {
    if (this.selectedItems.has(item)) {
      this.selectedItems.delete(item);
    } else {
      this.selectedItems.add(item);
    }
  }

  /**
   * Toggles the selection state of an inventory item.
   * @param {string} item - The name of the item to toggle.
   */
  toggleInventorySelection(item: string) {
    if (this.selectedItemsInInventory.has(item)) {
      this.selectedItemsInInventory.delete(item);
    } else {
      this.selectedItemsInInventory.add(item);
    }
  }

  /**
   * Toggles selection for all inventory items.
   */
  toggleAllInventorySelection() {
    if (this.selectedItemsInInventory.size === this.inventory.size) {
      this.selectedItemsInInventory.clear();
    } else {
      for(const i of this.inventory.keys()) {
        this.selectedItemsInInventory.add(i);
      }
    }
  }

  /**
   * Toggles the expanded details view for a specific item.
   * @param {string} item - The name of the item.
   */
  toggleDetails(item: string) {
    if (this.expandedItems.has(item)) {
      this.expandedItems.delete(item);
    } else {
      this.expandedItems.add(item);
      this.getItemDetails(item);
    }
  }

  /**
   * Handles search input changes and triggers filtering.
   */
  onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  /**
   * Filters available items based on the search term.
   */
  filterItems() {
    const query = this.searchTerm.toLowerCase().trim();

    this.filteredItems = Array.from(this.listOfItems).filter(n => n.toLowerCase().includes(query))
  }

  /**
   * Retrieves the current inventory from the server.
   */
  getInventory() {
    this.service.getInventory().subscribe(
      (result) => {
        for (let i of result) {
          this.inventory.set(i.name, { quantity: i.quantity, unit: i.unit });
        }

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
    this.api.getAllItems().subscribe(
      (result) => {
        for (let itemName of result) {
          if (!this.inventory.has(itemName)) this.listOfItems.add(itemName);
        }

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

    this.api.getSpecificItem(item).subscribe(
      (result) => {
        this.itemDetails = this.itemDetails.set(item, result);
      },
      (error) => {
        console.error(error);
        this.itemDetails = this.itemDetails.set(item, { name: "Não foi possível obter as informações do ingrediente: " + item, kcal: 0, kj: 0, protein: 0, carbohydrates: 0, lipids: 0, fibers: 0, category: 'none' });
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

    const input = event.target as HTMLInputElement
    let newValue = +(input).value;
    newValue = Math.max(0, Math.round(newValue * 100) / 100);
    if (newValue === inventoryItem.quantity) return;

    input.value = newValue.toFixed(2);
    inventoryItem.quantity = newValue;
    this.editedItems.add(item);
  }

  /**
   * Updates the unit of measurement for an inventory item.
   * @param {string} item - The name of the item.
   * @param {Event} event - The event from the input field.
   */
  updateUnit(item: string, event: Event) {
    const inventoryItem = this.inventory.get(item);
    if (!inventoryItem) return;

    const value = (event.target as HTMLInputElement).value.toLowerCase();
    inventoryItem.unit = value.slice(0, 3);
    this.editedItems.add(item);
  }

  /**
   * Adds selected items to the inventory.
   */
  addItemsToInventory() {
    const addedItems: Item[] = Array.from(this.selectedItems).map(n => { return { name: n, quantity: 1, unit: "" } });

    this.service.updateInventory(addedItems).subscribe(
      (result) => {
        const res: Set<string> = new Set(result.map(i => i.name));

        const successfullyAdded: string[] = [...this.selectedItems].filter(name => res.has(name));
        if (successfullyAdded.length !== this.selectedItems.size) console.error("Failed to add items!");

        for (let name of successfullyAdded) {
          this.inventory.set(name, { quantity: 1, unit: "" });
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
    const allItems = Array.from(this.inventory.keys());
    if (allItems.some(n => this.inventory.get(n)?.quantity === 0)) {
      this.openModal('deleteZeroQuantityModal');
    } else {
      this.updateItemsInInventory();
    }
  }

  /**
   * Updates the inventory with modified item quantities and units.
   * If `removeZeroQuantity` is true, removes items with zero quantity after updating.
   *
   * @param {boolean} [removeZeroQuantity=false] - Whether to remove items with zero quantity after updating.
   */
  updateItemsInInventory(removeZeroQuantity: boolean = false) {
    const updatedItems: Item[] = Array.from(this.editedItems).map(n => { return { name: n, quantity: this.inventory.get(n)?.quantity ?? 0, unit: this.inventory.get(n)?.unit ?? "" } });
    if (updatedItems.length === 0 && !removeZeroQuantity) return;

    this.service.updateInventory(updatedItems).subscribe(
      (result) => {
        const resultMap = new Map(result.map((item) => [item.name, { quantity: item.quantity, unit: item.unit }]));

        for (let item of this.editedItems) {
          const value = resultMap.get(item) ?? { quantity: 0, unit: "" };
          const current = this.inventory.get(item) ?? { quantity: 0, unit: "" };

          if (!value) this.inventory.delete(item);
          else {
            if (!current) this.inventory.set(item, value);
            else {
              if (value.quantity !== current.quantity) current.quantity = value.quantity;
              if (value.unit !== current.unit) current.unit = value.unit;
            }
          }
        }

        this.editedItems.clear();
        if (removeZeroQuantity) {
          const itemsToDelete: string[] = Array.from(this.inventory.keys()).filter(n => this.inventory.get(n)?.quantity === 0);
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
    const itemsToRemove: string[] = itemsToDelete ?? Array.from(this.selectedItemsInInventory);
    if (itemsToRemove.length === 0) return;

    this.service.removeInventory(itemsToRemove).subscribe(
      (result) => {
        const res = new Set(result.map(i => i.name));

        const deletedItems = itemsToRemove.filter(name => !res.has(name));
        if (deletedItems.length !== itemsToRemove.length) console.error("Failed to delete items!");

        for (let name of deletedItems) {
          this.listOfItems.add(name);
          this.inventory.delete(name);
        }

        if (itemsToDelete === null) this.selectedItemsInInventory.clear();
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
}
