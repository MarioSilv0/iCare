using backend.Models.Ingredients;

namespace backend.Models.Recipes
{
    public class Recipe
    {
        public int Id { get; set; }
        public string? Picture { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Area { get; set; }
        public string? YoutubeVideo { get; set; }
        public ICollection<RecipeIngredient>? RecipeIngredients { get; set; }
        public ICollection<UserRecipe>? UserRecipes { get; set; }
    }
}
