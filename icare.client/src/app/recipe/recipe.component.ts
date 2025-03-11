import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RecipeService } from '../services/recipes.service';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Recipe } from '../../models/recipe';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrl: './recipe.component.css',
})
export class RecipeComponent {

  public recipe: Recipe = {id: 100, picture: '', name: 'loading...', instructions: 'Please wait some minutes...', category: '', area: '', urlVideo: '', ingredients: [], isFavorite: false, calories: 0 }
  public safeVideoUrl: SafeResourceUrl = '';

  constructor(private http: HttpClient, private sanitizer: DomSanitizer, private route: ActivatedRoute, private api: RecipeService) { }

  ngOnInit() {
    this.getRecipe();
  }

  toggleFavorite() {
    if (this.recipe) {
      this.recipe.isFavorite = !this.recipe.isFavorite;
    }
  }

  getRecipe() {
    let name: string | null = this.route.snapshot.paramMap.get('name');
    if (name === null) return;

    this.api.getSpecificRecipe(name).subscribe(
      (result) => {
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
