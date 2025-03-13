export { Recipe, RecipeIngredient }

interface Recipe {
  id: number;
  picture: string;
  name: string;
  category: string,
  area: string,
  instructions: string;
  ingredients: RecipeIngredient[];
  urlVideo: string;
  isFavorite: boolean;
  calories: number;
}

interface RecipeIngredient {
  name: string;
  measure: string;
  grams: number;
}
