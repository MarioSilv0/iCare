import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UsersService } from '../services/users.service';
import { Goal } from '../../models';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  url = 'api/goal';
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

  private getUserGoal() {
      this.http.get<Goal>(this.url).subscribe({
          next: (goal) => {
              console.log(goal);
              this.userGoal = goal;
          },
          error: (error) => {
              console.error(error);
              this.userGoal = undefined;
              this.setupUserInfo();
          },
      });
  }

  private setupUserInfo() {
    this.userService.getUser().subscribe(
      ({ birthdate, height, weight, gender, activityLevel }) => {
        this.userInfoForm.patchValue({
          birthdate,
          weight,
          height,
          gender,
          activityLevel: activityLevel
        })

        const missingInfo = !birthdate || !weight || !height || !gender || !activityLevel;
        this.validInfo = !missingInfo;
      }
    )
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

    this.http.post(this.url, meta).subscribe({
      next: (res) => {
        console.log(res);
        this.snack.open('Meta criada com sucesso.', undefined, {
          duration: 2000,
          panelClass: ['success-snackbar'],
        });
        this.getUserGoal()
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
    return {
      goalType: goalType,
      autoGoalType: this.goalForm.value.selectedGoal,
      calories: this.goalForm.value.calories,
      startDate: this.goalForm.value.startDate,
      endDate: this.goalForm.value.endDate,
    } as Goal;
  }

  receiveData(data: DatesEmiter) {
    if (!data) return;

    this.goalForm.patchValue({
      startDate: data.startDate,
      endDate: data.endDate,
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

