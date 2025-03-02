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
    try {
      const storage = localStorage.getItem('user');
      if (storage) {
        this.notificationsPermission =
          JSON.parse(storage).notifications || true;
      }
    } catch (e) {
      console.error('Failed to get user data in localStorage:', e);
    }

    this.getInventory();
  }

  get inventoryArray() {
    return Array.from(this.inventory.entries());
  }

  get listOfItemsArray() {
    return Array.from(this.listOfItems);
  }

  toggleSelection(item: string) {
    if (this.selectedItems.has(item)) {
      this.selectedItems.delete(item);
    } else {
      this.selectedItems.add(item);
    }
  }

  toggleInventorySelection(item: string) {
    if (this.selectedItemsInInventory.has(item)) {
      this.selectedItemsInInventory.delete(item);
    } else {
      this.selectedItemsInInventory.add(item);
    }
  }

  toggleAllInventorySelection() {
    if (this.selectedItemsInInventory.size === this.inventory.size) {
      this.selectedItemsInInventory.clear();
    } else {
      for (const i of this.inventory.keys()) {
        this.selectedItemsInInventory.add(i);
      }
    }
  }

  toggleDetails(item: string) {
    if (this.expandedItems.has(item)) {
      this.expandedItems.delete(item);
    } else {
      this.expandedItems.add(item);
      this.getItemDetails(item);
    }
  }

  onSearchChange() {
    this.searchSubject.next(this.searchTerm);
  }

  filterItems() {
    const query = this.searchTerm.toLowerCase().trim();

    this.filteredItems = Array.from(this.listOfItems).filter((n) =>
      n.toLowerCase().includes(query)
    );
  }

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

  getItemDetails(item: string): void {
    if (this.itemDetails.has(item)) return;

    this.api.getSpecificItem(item).subscribe(
      (result) => {
        console.log({ result });
        this.itemDetails = this.itemDetails.set(item, result);
      },
      (error) => {
        console.error(error);
        this.itemDetails = this.itemDetails.set(item, {
          name: 'Não foi possível obter as informações do ingrediente: ' + item,
          kcal: 0,
          kj: 0,
          protein: 0,
          carbohydrates: 0,
          lipids: 0,
          fibers: 0,
          category: 'none',
        });
      }
    );
  }

  updateQuantity(item: string, event: Event): void {
    const inventoryItem = this.inventory.get(item);
    if (!inventoryItem) return;

    const input = event.target as HTMLInputElement;
    let newValue = +input.value;
    if (newValue === inventoryItem.quantity) return;

    // No negative numbers and rounded to 2 decimal places
    newValue = Math.max(0, newValue);
    newValue = Math.round(newValue * 100) / 100;

    input.value = newValue.toFixed(2);
    inventoryItem.quantity = newValue;
    this.editedItems.add(item);
  }

  updateUnit(item: string, event: Event) {
    const inventoryItem = this.inventory.get(item);
    if (!inventoryItem) return;

    const value = (event.target as HTMLSelectElement).value;
    inventoryItem.unit = value;
    this.updateMacroByUnit(item);
    this.editedItems.add(item);
    console.log({ inventoryItem });
  }

  addItemsToInventory() {
    const addedItems: Item[] = Array.from(this.selectedItems).map((n) => {
      return { name: n, quantity: 1, unit: '' };
    });

    this.service.updateInventory(addedItems).subscribe(
      (result) => {
        const res: Set<string> = new Set(result.map((i) => i.name));

        const successfullyAdded: string[] = [...this.selectedItems].filter(
          (name) => res.has(name)
        );
        if (successfullyAdded.length !== this.selectedItems.size)
          console.error('Failed to add items!');

        for (let name of successfullyAdded) {
          this.inventory.set(name, { quantity: 1, unit: '' });
          this.listOfItems.delete(name);
        }

        this.selectedItems.clear();
        this.filterItems();
        NotificationService.showNotification(
          this.notificationsPermission,
          addedItemNotification
        );
      },
      (error) => {
        console.error(error);
      }
    );
  }

  checkForEmptyItems() {
    const allItems = Array.from(this.inventory.keys());
    if (allItems.some((n) => this.inventory.get(n)?.quantity === 0)) {
      this.openModal('deleteZeroQuantityModal');
    } else {
      this.updateItemsInInventory();
    }
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
          const value = resultMap.get(item) ?? { quantity: 0, unit: '' };
          const current = this.inventory.get(item) ?? { quantity: 0, unit: '' };

          if (!value) this.inventory.delete(item);
          else {
            if (!current) this.inventory.set(item, value);
            else {
              if (value.quantity !== current.quantity)
                current.quantity = value.quantity;
              if (value.unit !== current.unit) current.unit = value.unit;
            }
          }
        }

        this.editedItems.clear();
        if (removeZeroQuantity) {
          const itemsToDelete: string[] = Array.from(
            this.inventory.keys()
          ).filter((n) => this.inventory.get(n)?.quantity === 0);
          this.removeItemFromInventory(itemsToDelete);
        }

        NotificationService.showNotification(
          this.notificationsPermission,
          editedItemNotification
        );
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
        if (deletedItems.length !== itemsToRemove.length)
          console.error('Failed to delete items!');

        for (let name of deletedItems) {
          this.listOfItems.add(name);
          this.inventory.delete(name);
        }

        if (itemsToDelete === null) this.selectedItemsInInventory.clear();
        this.filterItems();
        NotificationService.showNotification(
          this.notificationsPermission,
          removedItemNotification
        );
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

  updateMacroByUnit(item: string) {
    const inventory = this.inventory.get(item);
    const details = this.itemDetails.get(item);
    if (!inventory || !details || !inventory.unit) return;

    let { quantity, unit } = inventory;
    let { protein, carbohydrates, lipids, fibers, name, category, kcal, kj } =
      details;

    quantity =
      unit === 'kg'
        ? this.fromKilogramsToGrams(quantity)
        : this.fromGramsToKilograms(quantity);
    this.inventory.set(item, { quantity, unit });
    let ing: Ingredient;

    if (unit === 'g') {
      ing = {
        name,
        category,
        kcal,
        kj,
        protein: (protein * quantity) / 100,
        carbohydrates: (carbohydrates * quantity) / 100,
        lipids: (lipids * quantity) / 100,
        fibers: (fibers * quantity) / 100,
      };
    } else {
      ing = {
        name,
        category,
        kcal,
        kj,
        protein: (protein * quantity) / 0.1,
        carbohydrates: (carbohydrates * quantity) / 0.1,
        lipids: (lipids * quantity) / 0.1,
        fibers: (fibers * quantity) / 0.1,
      };
      this.itemDetails.set(item, ing);
    }
  }

  private fromKilogramsToGrams(quantity: number): number {
    return quantity * 1000;
  }
  private fromGramsToKilograms(quantity: number): number {
    return quantity / 1000;
  }
}
