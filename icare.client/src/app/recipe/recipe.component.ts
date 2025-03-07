import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RecipeService, Recipe } from '../services/recipes.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrl: './recipe.component.css',
})
export class RecipeComponent {
  public recipe: Recipe = { picture: '', name: 'loading...', description: 'Please wait some minutes...', category: '', area: '', urlVideo: '', ingredients: [], isFavorite: false, calories: 0 }

  constructor(private http: HttpClient, private route: ActivatedRoute, private api: RecipeService) { }

  ngOnInit() {
    this.getRecipe();
  }

  toggleFavorite() {
    if(!this.recipe) return

    let old = this.recipe.isFavorite;

    try {
      let url = ''
      this.http.post(url, {
        recipe: this.recipe,
        isFavorite: this.recipe.isFavorite ? 1 : 0
      })
      this.recipe.isFavorite = !this.recipe.isFavorite;
    } catch (e) {
      console.error(e);
      this.recipe.isFavorite = old
    }
  }

  getRecipe() {
    let name: string | null = this.route.snapshot.paramMap.get('name');
    if (name === null) return;

    this.api.getSpecificRecipe(name).subscribe(
      (result) => {
        console.log(result)
        this.recipe = { ...result, ingredients: result.ingredients};
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
