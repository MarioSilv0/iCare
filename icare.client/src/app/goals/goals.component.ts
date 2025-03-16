import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  goalType: string = 'Manual';
  userInfoForm: FormGroup;
  goalForm: FormGroup;
  userGoals?: Goal[];
  goals: Goal[] = [
    { name: 'Perder Peso' },
    { name: 'Manter Peso' },
    { name: 'Ganhar Peso' },
  ];
  constructor(
    private http: HttpClient,
    private snack: MatSnackBar,
    private fb: FormBuilder
  ) {
    this.goalForm = this.fb.group({
      selectedGoal: [''],
      calories: [0],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });

    this.userInfoForm = this.fb.group({
      birthdate: ['', Validators.required],
      weigth: [
        0,
        [Validators.required, Validators.min(0), Validators.max(700)],
      ],
      height: [0, [Validators.required, Validators.min(0), Validators.max(3)]],
      gender: ['', Validators.required],
      activity: ['', Validators.required],
    });
  }

  ngOnInit() {
    let url = 'api/goal/current';
    this.http.get<Goal[]>(url).subscribe({
      next: (goals) => (this.userGoals = goals),
      error: (error) => {
        console.log(error);
        this.userGoals = [];
      },
    });
  }

  toggleGoalType() {
    this.goalType = this.goalType === 'Automática' ? 'Manual' : 'Automática';
  }

  updateUserInfo() {
    let url = '/api/user/physical';
    this.http.put(url, this.userInfoForm.value).subscribe({
      next: () => {
        this.snack.open('Informações atualizadas com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
      },
      error: () => {
        this.snack.open('Erro ao tentar atualizar informações.', undefined, {
          duration: 2000,
          panelClass: ['fail-snackbar'],
        });
      },
    });
  }

  addGoal() {
    let meta = this.createGoal(this.goalType);

    let url = 'api/goal/create';
    this.http.post(url, meta).subscribe({
      next: () => {
        this.snack.open('Meta criada com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
      },
      error: () => {
        this.snack.open('Erro ao tentar criar meta.', undefined, {
          duration: 2000,
          panelClass: ['fail-snackbar'],
        });
      },
      complete: () => {
        this.goalForm.reset();
      },
    });
  }

  createGoal(goalType: string) {
    let goal;
    switch (goalType) {
      case 'Automática':
        {
          goal = {
            type: 'Automatica',
            goal: this.goalForm.value.selectedGoal,
          } as MetaAutomatica;
        }
        break;
      case 'Manual':
        {
          goal = {
            type: 'Manual',
            calories: this.goalForm.value.calories,
            startDate: this.goalForm.value.startDate,
            endDate: this.goalForm.value.endDate,
          } as MetaManual;
        }
        break;
      default: {
        console.error('O tipo de meta não existe.');
        return null;
      }
    }
    return goal;
  }

  receiveData(data: DatesEmiter) {
    if (!data) return;

    this.goalForm.patchValue({
      startDate: data.startDate,
      endDate: data.endDate,
    });
  }
}

interface Goal {
  name: string;
}

interface DatesEmiter {
  startDate: string;
  endDate: string;
}

interface Meta {
  type: string;
}

interface MetaManual extends Meta {
  calories: number;
  startDate: string;
  endDate: string;
}
interface MetaAutomatica extends Meta {
  goal: string;
}
