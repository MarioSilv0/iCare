import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-goal',
  templateUrl: './goal.component.html',
  styleUrl: './goal.component.css'
})
export class GoalComponent {
  @Input() goal: UserGoal = { type: '', dailyCalories: 0, duration: 0 };
  //@Input() goal: GoalDTO = { goalType: '', autoGoalType: '', calories: 0, startDate: '', endDate: '', };

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

//interface GoalDTO {
//  goalType: string,
//  autoGoalType: string,
//  calories: number,
//  startDate: string,
//  endDate: string,
//}
