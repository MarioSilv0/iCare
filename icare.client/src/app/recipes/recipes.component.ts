import { Component } from '@angular/core';

@Component({
  selector: 'app-recipes',
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css',
})
export class RecipesComponent {
  public filters: string[] = [
    'Inventário',
    'Meta Alimentar',
    'Restrições Alimentares',
    'Preferências Alimentares',
    'Favoritos',
  ];
  public recipes: {
    img: string;
    title: string;
    description: string;
    isFavorite: boolean;
  }[] = [
    {
      img: '',
      title: 'titulo da receita',
      description: 'calorias: 700 kcal',
      isFavorite: false,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
    {
      img: '',
      title: 'titulo da receita2',
      description: 'calorias: 760 kcal',
      isFavorite: true,
    },
  ];
  public searchTerm: string = '';

  constructor() {}

  ngOnInit() {
    console.log('HELP ME!');
  }

  toggleFavoriteRecipe(id: number) {
    this.recipes[id].isFavorite = !this.recipes[id].isFavorite;
  }

  searchRecipes(searchTerm: string) {
    const filteredRecipes = this.recipes.forEach((r) => {
      r.title.includes(searchTerm) || r.description.includes(searchTerm);
    });
    console.log(filteredRecipes);
    // renderizar a nova lista
    // guardar a antiga lista de receitas
  }
}
