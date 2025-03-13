import { Component, EventEmitter, Output } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css'
})
export class CalendarComponent {

  @Output() dates = new EventEmitter<DatesEmiter>();

  startDate: string = '';
  endDate: string = '';



  constructor(private snackBar: MatSnackBar) { }

  validateDate() {
    if (!this.startDate || !this.endDate) return;

    const start = new Date(this.startDate);
    const end = new Date(this.endDate);

    if (end < start) {
      this.startDate = '';
      this.endDate = '';
      this.snackBar.open('A data final deve ser maior que a data inicial.', '', {
        duration: 2000,
        panelClass: ["fail-snackbar"]
      });
    }
    this.dates.emit({
      startDate: this.startDate,
      endDate: this.endDate
    })
  }

}

interface DatesEmiter {
  startDate: string,
  endDate: string
}
