import { Component } from '@angular/core';
import { UsersService } from '../services/users.service';
import { ApiService, Item } from '../services/api.service';

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

  addItemsToInventory() {
    console.log(this.selectedItems)
    const tmp: Item[] = Array.from(this.selectedItems).map(n => { return { name: n, quantity: 1 } });

    this.service.updateInventory(tmp).subscribe(
      (result) => {
        for (let i of result) {
          this.inventory.set(i.name, i.quantity);
          this.listOfItems.delete(i.name);
        }

        this.selectedItems.clear();
      },
      (error) => {
        console.error(error);
      }
    );
  }

  updateItemsInInventory() {
    this.service.updateInventory([]).subscribe(
      (result) => {
        console.log(result);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  removeItemFromInventory() {
    this.service.removeInventory([]).subscribe(
      (result) => {
        this.inventory.clear();
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
}
