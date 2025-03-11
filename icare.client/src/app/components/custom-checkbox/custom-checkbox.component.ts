import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'custom-checkbox',
  templateUrl: './custom-checkbox.component.html',
  styleUrls: ['./custom-checkbox.component.css']
})
export class CustomCheckboxComponent {
  @Input() checked: boolean = false;
  @Input() label: string = '';
  @Input() id: string = '';
  @Output() checkedChange = new EventEmitter<boolean>();

  onToggle(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.checkedChange.emit(input.checked);
  }
}
