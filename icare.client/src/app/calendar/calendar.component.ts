import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css'
})
export class CalendarComponent {
  @Output() dates = new EventEmitter<DatesEmiter>();

  dateForm = new FormGroup({
    startDate: new FormControl(''),
    endDate: new FormControl('')
  })
  constructor(private snackBar: MatSnackBar) { }

  validateDate() {
    if (!this.dateForm.value.startDate || !this.dateForm.value.endDate) return;

    const start = new Date(this.dateForm.value.startDate);
    const end = new Date(this.dateForm.value.endDate);
    const today = new Date();

    console.log({ start, end, today });

    this.dates.emit({
      startDate: this.dateForm.value.startDate,
      endDate: this.dateForm.value.endDate
    });

    if (start.getDate() < today.getDate()) {
      this.snackBar.open("Só pode criar metas com inicio atual ou superior.", undefined, {
        duration: 2000,
        panelClass: ['fail-snackbar']
      });
      this.dateForm.reset();  
      return;
    }

    if (end.getDate() < start.getDate()) {
      this.snackBar.open('A data final deve ser maior que a data inicial.', undefined, {
        duration: 2000,
        panelClass: ["fail-snackbar"],
      });
      this.dateForm.reset();  
      return;
    }
  }
}

interface DatesEmiter {
  startDate: string,
  endDate: string
}
