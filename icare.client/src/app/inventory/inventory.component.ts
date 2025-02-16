import { Component } from '@angular/core';
import { UsersService, Item } from '../services/users.service';

@Component({
  selector: 'app-inventory',
  standalone: false,

  templateUrl: './inventory.component.html',
  styleUrl: './inventory.component.css'
})

export class InventoryComponent {
  public inventory: Item[] = [{name: "Banana", quantity: 1}];

  constructor(private service: UsersService) { }

  ngOnInit() {
    this.getInventory();
  }

  getInventory() {
    this.service.getInventory().subscribe(
      (result) => {
        this.inventory = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
