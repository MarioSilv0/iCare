import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  goalType: string;
  userGoals?: Goal[]
  goals: Goal[] = [
    { name: 'Perder Peso' },
    { name: 'Manter Peso' },
    { name: 'Ganhar Peso' },
  ]; // ler da base de dados
  selectedGoal: string = ''
  calories: number = 0
  startDate: string = ''
  endDate: string = ''
  constructor(private http: HttpClient, private snack: MatSnackBar) {
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
      meta["calories"] = this.calories
      meta["start"] = this.startDate
      meta["end"] = this.endDate
    }

    try {
      let url = '' // discutir endpoint
      this.http.post(url, meta)
      console.log(meta)
      this.snack.open("Meta criada com sucesso.", undefined, {
        duration: 2000,
        panelClass: ['success-snackbar']
      })
    } catch (e) {
      this.snack.open("Erro ao tentar criar meta.", undefined, {
        duration: 2000,
        panelClass: ['fail-snackbar']
      })
    } finally {
      this.selectedGoal = ''
      this.calories = 0
      this.startDate = ''
      this.endDate = ''
    }

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
