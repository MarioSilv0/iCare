export { Recipe, IngredientRecipe }

interface Recipe {
  id: number;
  picture: string;
  name: string;
  category: string,
  area: string,
  instructions: string[];
  ingredients: IngredientRecipe[];
  urlVideo: string;
  isFavorite: boolean; 
}

interface IngredientRecipe {
  name: string;
  measure: string;
  grams: number;
}
