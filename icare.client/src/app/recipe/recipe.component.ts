import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RecipeService, Recipe } from '../services/recipes.service';
import { ActivatedRoute } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrl: './recipe.component.css',
})
export class RecipeComponent {
  public recipe: Recipe = { picture: '', name: 'loading...', description: 'Please wait some minutes...', category: '', area: '', youtubeVideo: '', ingredients: [], isFavorite: false, calories: 0 }
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
        console.log("Recipe Data:", result);

        this.recipe = { ...result, ingredients: result.ingredients };
        this.safeVideoUrl = this.getEmbeddedVideoUrl(this.recipe.youtubeVideo);
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
