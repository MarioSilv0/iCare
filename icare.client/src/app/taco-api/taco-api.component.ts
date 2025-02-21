import { Component, OnInit } from '@angular/core';
import { TacoApiService } from '../services/taco-api.service';

@Component({
  selector: 'app-taco-api',
  templateUrl: './taco-api.component.html',
  styleUrl: './taco-api.component.css'
})
export class TacoApiComponent implements OnInit {
  searchTerm: string = '';
  results: any[] = [];
  loading: boolean = false;
  error: string | null = null;

  constructor(private tacoApiService: TacoApiService) { }

  ngOnInit(): void {
    // Opcional: carregar todos os ingredientes ao iniciar
    this.loadAllIngredients();
  }

  searchFood(): void {
    if (!this.searchTerm.trim()) return;

    this.loading = true;
    this.error = null;

    this.tacoApiService.searchFood(this.searchTerm)
      .subscribe({
        next: (response) => {
          this.results = response.data.getFoodByName;
          this.loading = false;
        },
        error: (err) => {
          console.error('Erro na busca:', err);
          this.error = 'Erro ao buscar alimentos. Verifique se a API está funcionando.';
          this.loading = false;
        }
      });
  }

  loadAllIngredients(): void {
    this.loading = true;

    this.tacoApiService.getAllFood()
      .subscribe({
        next: (response) => {
          this.results = response.data.getAllFood;
          this.loading = false;
        },
        error: (err) => {
          console.error('Erro ao carregar ingredientes:', err);
          this.error = 'Erro ao carregar a lista de ingredientes.';
          this.loading = false;
        }
      });
  }
}
