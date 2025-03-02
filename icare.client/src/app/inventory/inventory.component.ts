import { Component } from '@angular/core';
import { debounceTime, Subject } from 'rxjs';
import { ApiService, Ingredient } from '../services/api.service';
import {
  addedItemNotification,
  editedItemNotification,
  NotificationService,
  removedItemNotification,
} from '../services/notifications.service';
import { Item, UsersService } from '../services/users.service';

declare var bootstrap: any;

@Component({
  selector: 'app-inventory',
  standalone: false,

  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css',
})
export class InventoryComponent {
  private notificationsPermission: boolean = true;
  public searchTerm: string = '';
  public searchSubject = new Subject<string>();

  public inventory: Map<string, { quantity: number; unit: string }> = new Map();
  public listOfItems: Set<string> = new Set();
  public filteredItems: string[] = [];

  public selectedItems: Set<string> = new Set();
  public selectedItemsInInventory: Set<string> = new Set();
  public expandedItems: Set<string> = new Set();
  public editedItems = new Set<string>();

  public itemDetails: Map<string, Ingredient> = new Map();

  public units: string[] = ['g', 'kg'];

  constructor(private service: UsersService, private api: ApiService) {
    this.searchSubject
      .pipe(debounceTime(300))
      .subscribe(() => this.filterItems());
  }

  ngOnInit() {
    this.loadNotificationPreferences();
    this.getInventory();
  }

  private loadNotificationPreferences() {
    try {
      const storage = localStorage.getItem('user');
      if (storage) {
        this.notificationsPermission = JSON.parse(storage).notifications ?? true;
      }
    } catch (e) {
      console.error('Failed to get user data in localStorage:', e);
    }
  }

  get inventoryArray() {
    return Array.from(this.inventory.entries());
  }

  get listOfItemsArray() {
    return Array.from(this.listOfItems);
  }

  toggleSelection(item: string) {
    this.selectedItems.has(item) ? this.selectedItems.delete(item) : this.selectedItems.add(item);
  }

  toggleInventorySelection(item: string) {
    this.selectedItemsInInventory.has(item) ? this.selectedItemsInInventory.delete(item) : this.selectedItemsInInventory.add(item);
  }

  toggleAllInventorySelection() {
    this.selectedItemsInInventory.size === this.inventory.size
      ? this.selectedItemsInInventory.clear()
      : this.inventory.forEach((_, key) => this.selectedItemsInInventory.add(key));
  }

  toggleDetails(item: string) {
    this.expandedItems.has(item) ? this.expandedItems.delete(item) : this.expandedItems.add(item);
    if (!this.itemDetails.has(item)) {
      this.getItemDetails(item);
    }
  }

  onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  filterItems() {
    const query = this.searchTerm.toLowerCase().trim();
    this.filteredItems = Array.from(this.listOfItems).filter((n) => n.toLowerCase().includes(query));
  }

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

  getListItems() {
    this.api.getAllItems().subscribe(
      (result) => {
        result.forEach((itemName) => { if (!this.inventory.has(itemName)) this.listOfItems.add(itemName); });
        this.filterItems();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  getItemDetails(item: string): void {
    if (this.itemDetails.has(item)) return;

    this.api.getSpecificItem(item).subscribe(
      (result) => {
        console.log({ ...result  });
        let info = this.inventory.get(item) ?? { quantity: 1, unit: "g" };
        if (!info.unit) info.unit = "g";
        console.log(info.unit)

        let total: number = info.unit === "g" ? 100 : 0.1;
        console.log(total)

        this.convertIngredientUnits(result, 'g', info.unit);
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

  checkForEmptyItems() {
    const hasEmptyItems = Array.from(this.inventory.values()).some((item) => item.quantity === 0);
    hasEmptyItems ? this.openModal('deleteZeroQuantityModal') : this.updateItemsInInventory();
  }

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

  openModal(id: string) {
    const modalElement = document.getElementById(id);
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

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
