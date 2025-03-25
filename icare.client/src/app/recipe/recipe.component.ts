import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Recipe } from '../../models/recipe';
import { RecipeService } from '../services/recipes.service';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrl: './recipe.component.css',
})
export class RecipeComponent {

  public recipe: Recipe = { id: 100, picture: '', name: 'loading...', instructions: 'Please wait some minutes...', category: '', area: '', urlVideo: '', ingredients: [], isFavorite: false, calories: -1, proteins: -1, carbohydrates: -1, lipids: -1, fibers: -1 }
  public safeVideoUrl: SafeResourceUrl = '';

  constructor(private http: HttpClient, private sanitizer: DomSanitizer, private route: ActivatedRoute, private recipeService: RecipeService) { }

  ngOnInit() {
    this.getRecipe();
  }

  toggleFavorite() {
    if(!this.recipe) return

    let old = this.recipe.isFavorite;
    this.recipeService.toggleFavorite(this.recipe.name).subscribe(
      (result) => this.recipe.isFavorite = !old,
      (error) => {
        console.error(error);
        this.recipe.isFavorite = old
      }
    )
  }

  getRecipe() {
    let name: string | null = this.route.snapshot.paramMap.get('name');
    if (name === null) return;

    this.recipeService.getSpecificRecipe(name).subscribe(
      (result) => {
        console.log(result);
        this.recipe = { ...result, ingredients: result.ingredients };
        this.safeVideoUrl = this.getEmbeddedVideoUrl(this.recipe.urlVideo);
      },
      (error) => {
        console.error(error);
      }
    );
  }

  /**
    * Converts a YouTube URL into a safe embed URL.
    */
  getEmbeddedVideoUrl(url: string): SafeResourceUrl {
    if (!url) return '';

    const embedUrl = url.replace("watch?v=", "embed/");
    return this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl);
  }
}
