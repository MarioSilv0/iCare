import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../services/users.service';
import { Goal } from '../../models';

declare var bootstrap: any;

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  userGoal?: Goal;
  validInfo = false;

  goalType: string = 'Manual';
  userInfoForm: FormGroup;
  goalForm: FormGroup;
  goals: AutoGoalType[] = [
    { name: 'Perder Peso' },
    { name: 'Manter Peso' },
    { name: 'Ganhar Peso' },
  ];
  genders = [
    { value: '', label: 'Selecione o seu genero'},
    { value: 'Male', label: 'Masculino'},
    { value: 'Female', label: 'Feminino'}
  ]

  activities = [
    {value: '', label: 'Nivel de atividade'},
    {value: 'Sedentary', label: 'Sedentário'},
    {value: 'Lightly Active', label: 'Pouco Ativo'},
    {value: 'Moderately Active', label: 'Moderadamente Ativo'},
    {value: 'Very Active', label: 'Muito Ativo'},
    {value: 'Super Active', label: 'Super Ativo'}
  ]
  constructor(
    private http: HttpClient,
    private snack: MatSnackBar,
    private fb: FormBuilder,
    private userService: UsersService
  ) {
    this.goalForm = this.fb.group({
      selectedGoal: [''],
      calories: [2000],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });

    this.userInfoForm = this.fb.group({
      birthdate: ['', Validators.required],
      weight: [
        0,
        [Validators.required, Validators.min(0), Validators.max(700)],
      ],
      height: [0, [Validators.required, Validators.min(0), Validators.max(3)]],
      gender: ['', Validators.required],
      activityLevel: ['', Validators.required],
    });    
  }

  ngOnInit() {
    this.getUserGoal();
  }

  getUserGoal() {
    let url = 'api/goal';
    this.http.get<Goal>(url).subscribe({
      next: (goal) => {
        if (!goal || Object.keys(goal).length === 0) {
          this.userGoal = undefined;
          this.validInfo = false;
          this.setupUserInfo();
        } else {
          this.userGoal = goal;
          this.validInfo = true;
        }
       },
        error: () => {
          this.userGoal = undefined;
          this.validInfo = false
          this.setupUserInfo();
      }
    });
    
  }

  setupUserInfo() {
    this.userService.getUser().subscribe(
      ({ birthdate, height, weight, gender, activityLevel }) => {
        this.userInfoForm.patchValue({
          birthdate,
          weight,
          height,
          gender,
          activityLevel
        })

        const missingInfo = !birthdate || !weight || !height || !gender || !activityLevel;
        this.validInfo = !missingInfo;
      }
    )
  }

  /**
   * Opens a Bootstrap modal by its ID.
   * @param {string} id - The ID of the modal to be opened.
   */
  openModal(id: string) {
    const modalElement = document.getElementById(id);
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  toggleValidInfo() {
    this.validInfo = !this.validInfo;
  }

  toggleGoalType() {
    this.goalType = this.goalType === 'Automática' ? 'Manual' : 'Automática';
  }

  updateUserInfo() {
    let url = '/api/user/physical';

    let userPhysical = {
      Birthdate: this.userInfoForm.get("birthdate")?.value,
      Height: this.userInfoForm.get("height")?.value,
      Weight: this.userInfoForm.get("weight")?.value,
      Gender: this.userInfoForm.get("gender")?.value,
      ActivityLevel: this.userInfoForm.get("activityLevel")?.value,
    }

    this.http.put(url, userPhysical).subscribe({
      next: (d) => {
        console.log(d)
        this.snack.open('Informações atualizadas com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
        this.validInfo = true;
      },
      error: (e) => {
        console.log(e)
        this.snack.open('Erro ao tentar atualizar informações.', undefined, {
          duration: 2000,
          panelClass: ['fail-snackbar'],
        });
      },
    });
  }

  addGoal() {
    let meta = this.createGoal(this.goalType);
    let url = 'api/goal';

    this.http.post(url, meta).subscribe({
      next: (res) => {
        console.log(res);
        this.snack.open('Meta criada com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
        this.toggleValidInfo();
        this.getUserGoal();
      },
      error: (err) => {
        console.log(err);
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
    if(goalType == "Automática" || goalType == "Manual"){
    return {
      goalType: goalType,
      autoGoalType: this.goalForm.value.selectedGoal,
      calories: this.goalForm.value.calories,
      startDate: this.goalForm.value.startDate,
      endDate: this.goalForm.value.endDate,
      } as Goal;
    }
    else{
    console.error('O tipo de meta não existe.');
      return null;
    }
  }

  receiveData(data: DatesEmiter) {
    if (!data) return;

    this.goalForm.patchValue({
      startDate: data.startDate,
      endDate: data.endDate,
    });
  }

  updateGoal() {
    let url = 'api/goal';
    this.http.put(url, this.userGoal).subscribe({
      next: (res) => {
        console.log(res);
        this.snack.open('Meta editada com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
        this.validInfo = true;
      },
      error: (err) => {
        console.log(err);
        this.snack.open('Erro ao tentar editar meta.', undefined, {
          duration: 2000,
          panelClass: ['fail-snackbar'],
        });
      },
    });
  }

  removeGoal() {
    let url = 'api/goal';
    this.http.delete(url).subscribe({
      next: (res) => {
        console.log(res);
        this.snack.open('Meta excluida com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
        this.toggleValidInfo();
        this.getUserGoal();
      },
      error: (err) => {
        console.log(err);
        this.snack.open('Erro ao tentar excluir meta.', undefined, {
          duration: 2000,
          panelClass: ['fail-snackbar'],
        });
      },
    });
  }

}

interface AutoGoalType {
  name: string;
}

interface DatesEmiter {
  startDate: string;
  endDate: string;
}

