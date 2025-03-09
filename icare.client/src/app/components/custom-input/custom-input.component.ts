import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'custom-input',
  templateUrl: './custom-input.component.html',
  styleUrls: ['./custom-input.component.css']
})
export class CustomInputComponent {
  @Input() containerStyles: { [key: string]: string } = {};
  @Input() inputStyles: { [key: string]: string } = {};
  @Input() type: string = 'text';
  @Input() placeholder: string = '';
  @Input() value: string | number = '';
  @Input() id: string = '';
  @Input() name: string = '';
  @Input() disabled: boolean = false;
  @Output() valueChange = new EventEmitter<string | number>();

  onInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;

    let newValue: string | number = input.value;
    if (this.type === 'number') newValue = parseFloat(input.value);

    this.valueChange.emit(newValue);
  }
}
