import { Component } from '@angular/core';

@Component({
  selector: 'app-goals',
  templateUrl: './goals.component.html',
  styleUrl: './goals.component.css',
})
export class GoalsComponent {
  goalType: string;
  goals: Goals[] = [
    { name: 'Perder Peso' },
    { name: 'Manter Peso' },
    { name: 'Ganhar Peso' },
  ]; // ler da base de dados
  constructor() {
    this.goalType = 'Automática';
  }

  toggleGoalType() {
    this.goalType = this.goalType === 'Automática' ? 'Manual' : 'Automática';
  }
}

interface Goals {
  name: string;
}
