import { Component } from '@angular/core';
import { UsersService } from '../services/users.service';
import { ApiService, Ingredient, Item } from '../services/api.service';

declare var bootstrap: any;

@Component({
  selector: 'app-inventory',
  standalone: false,

  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css'
})

export class InventoryComponent {
  public inventory: Map<string, number> = new Map();
  public listOfItems: Set<string> = new Set();

  public selectedItems: Set<string> = new Set();
  public selectedItemsInInventory: Set<string> = new Set();
  public expandedItems: Set<string> = new Set();
  public editedItems = new Set<string>();

  public itemDetails: Map<string, Ingredient> = new Map();

  constructor(private service: UsersService, private api: ApiService) { }

  ngOnInit() {
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
      for(const i of this.inventory.keys()) {
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

  getInventory() {
    this.service.getInventory().subscribe(
      (result) => {
        for (let i of result) {
          this.inventory.set(i.name, i.quantity);
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
        const tmp = [{ name: "Banana" }, { name: "Potato" }, { name: "Apple" }, { name: "Carrot" }]
        for (let i of tmp) {
          if (!this.inventory.has(i.name)) this.listOfItems.add(i.name);
        }
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
        const tmp: Ingredient = { name: "Apple", nutrients: { kcal: 124, kJ: 517, protein: 2.6, carbohydrates: 25.8 }, category: { name: "Fruit" } };

        this.itemDetails = this.itemDetails.set(item, tmp);
      },
      (error) => {
        console.error(error);
        this.itemDetails = this.itemDetails.set(item, { name: "Não foi possível obter as informações do ingrediente: " + item, nutrients: { kcal: 0, kJ: 0, protein: 0, carbohydrates: 0 }, category: { name: 'none' } });
      }
    );
  }

  updateQuantity(item: string, event: Event): void {
    const input = event.target as HTMLInputElement
    let newValue = +(input).value;
    if (newValue === this.inventory.get(item)) return;

    // No negative numbers and rounded to 2 decimal places
    newValue = Math.max(0, newValue);
    newValue = Math.round(newValue * 100) / 100;

    input.value = newValue.toFixed(2);
    this.inventory.set(item, newValue);
    this.editedItems.add(item);
  }

  addItemsToInventory() {
    const addedItems: Item[] = Array.from(this.selectedItems).map(n => { return { name: n, quantity: 1 } });

    this.service.updateInventory(addedItems).subscribe(
      (result) => {
        const res: Set<string> = new Set(result.map(i => i.name));

        const successfullyAdded: string[] = [...this.selectedItems].filter(name => res.has(name));
        if (successfullyAdded.length !== this.selectedItems.size) console.error("Failed to add items!");

        for (let name of successfullyAdded) {
          this.inventory.set(name, 1);
          this.listOfItems.delete(name);
        }

        this.selectedItems.clear();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  updateItemsInInventory() {
    const updatedItems: Item[] = Array.from(this.editedItems).map(n => { return { name: n, quantity: this.inventory.get(n) ?? 0 } });
    if (updatedItems.length === 0) return;

    this.service.updateInventory(updatedItems).subscribe(
      (result) => {
        const resultMap = new Map(result.map((item) => [item.name, item.quantity]));

        for (let item of this.editedItems) {
          const value = resultMap.get(item) ?? 0;
          if (value !== this.inventory.get(item)) this.inventory.set(item, value);
        }

        this.editedItems.clear();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  removeItemFromInventory() {
    const deletedItems: string[] = Array.from(this.selectedItemsInInventory);

    this.service.removeInventory(deletedItems).subscribe(
      (result) => {
        const res = new Set(result.map(i => i.name));

        const deletedItems = [...this.selectedItemsInInventory].filter(name => !res.has(name));
        if (deletedItems.length !== this.selectedItemsInInventory.size) console.error("Failed to delete items!");

        for (let name of deletedItems) {
          this.listOfItems.add(name);
          this.inventory.delete(name);
        }

        this.selectedItemsInInventory.clear();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  openDeleteModal() {
    const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
    modal.show();
  }
}
