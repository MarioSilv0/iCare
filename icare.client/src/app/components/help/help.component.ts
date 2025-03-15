import { Component, Input } from '@angular/core';

@Component({
  selector: 'help',
  templateUrl: './help.component.html',
  styleUrl: './help.component.css'
})
export class HelpComponent {
  @Input() helpText: string = '';
}
