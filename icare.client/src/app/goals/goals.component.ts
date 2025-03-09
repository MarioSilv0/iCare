import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  goalType: string;
  calorias: number = 0
  userGoals?: Goal[]
  goals: Goal[] = [
    { name: 'Perder Peso' },
    { name: 'Manter Peso' },
    { name: 'Ganhar Peso' },
  ]; // ler da base de dados
  selectedGoal: string = ''
  startDate: string = ''
  endDate: string = ''
  constructor(private http: HttpClient) {
    this.goalType = 'Manual';
  }

  ngOnInit() {
    let url = ''
    try {
      this.http.get<Goal[]>(url).subscribe(
        goals => {
          this.userGoals = goals
        }
      )
    } catch (e) {
      console.error(e)
    }
  }

  toggleGoalType() {
    this.goalType = this.goalType === 'Automática' ? 'Manual' : 'Automática';
  }

  addGoal() {
    let meta: any = { type: this.goalType }
    if (this.goalType === 'Automática') {
      if (!this.selectedGoal) return
      meta["goal"] = this.selectedGoal.replace(" ","-")
    } else {
      meta["calories"] = this.calorias
      meta["start"] = this.startDate
      meta["end"] = this.endDate
    }
    alert(`meta adicionada ${JSON.stringify(meta)}`)
  }

  receiveData(data: DatesEmiter) {
    this.startDate = data.startDate
    this.endDate = data.endDate
  }
}

interface Goal {
  name: string;
}

interface DatesEmiter {
  startDate: string,
  endDate: string,
}
