import { Component, Input } from '@angular/core';
import { Goal } from '../../models';

@Component({
  selector: 'app-goal',
  templateUrl: './goal.component.html',
  styleUrl: './goal.component.css'
})
export class GoalComponent {
  @Input() goal: Goal = { goalType: '', autoGoalType: '', calories: 0, startDate: '', endDate: '', };

  constructor() { }

  objectEntries(obj: any): [string, any][] {
    return Object.entries(obj);
  }
}

