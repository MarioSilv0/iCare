import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-goal',
  templateUrl: './goal.component.html',
  styleUrl: './goal.component.css'
})
export class GoalComponent {
  @Input() goal: UserGoal = { type: '', dailyCalories: 0, duration: 0 };

  constructor() { }

  objectEntries(obj: any): [string, any][] {
    return Object.entries(obj);
  }
}

export interface UserGoal {
  type: string,
  dailyCalories: number,
  duration: number,
}
