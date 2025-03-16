import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-goal',
  templateUrl: './goal.component.html',
  styleUrl: './goal.component.css'
})
export class GoalComponent {
  @Input() goal: UserGoal | undefined;

  constructor() {
    
  }
}

export interface UserGoal {
  type: string,
  dailyCalories: number,
  duration: number,
}
