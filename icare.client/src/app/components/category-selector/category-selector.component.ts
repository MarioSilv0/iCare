import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'category-selector',
  templateUrl: './category-selector.component.html',
  styleUrls: ['./category-selector.component.css']
})
export class CategorySelectorComponent {
  @Input() listId: string = '';
  @Input() placeholder: string = "";
  @Input() options: string[] = [];
  @Input() selectedItems: Set<string> = new Set();
  @Output() addItem = new EventEmitter<string>();
  @Output() removeItem = new EventEmitter<string>();

  onAdd(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value.trim() as string;
    if (value) {
      this.addItem.emit(value);
      input.value = '';
    }
  }

  onRemove(item: string): void {
    this.removeItem.emit(item);
  }
}
