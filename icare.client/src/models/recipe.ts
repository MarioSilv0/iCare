export { Recipe, IngredientRecipe }

interface Recipe {
  id: number;
  title: string;
  instructions: [];
  thumb: string;
  video: string;
  ingredients: IngredientRecipe[];
  isFavorite: boolean; //não faz sentido
}

interface IngredientRecipe {
  name: string;
  measure: string;
  grams: string;
}
