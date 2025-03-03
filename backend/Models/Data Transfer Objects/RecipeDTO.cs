using backend.Models.Recipes;

namespace backend.Models.Data_Transfer_Objects
{
    public class RecipeDTO
    {
        
        public string? Picture { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Area { get; set; }

        public string? YoutubeVideo { get; set; }

        public IEnumerable<ItemDTO>? RecipeIngredients { get; set; }

        public bool IsFavorite { get; set; }

        public float Calories { get; set; }

        public RecipeDTO(Recipe recipe, bool wantDetails, string userId)
        {
            if (recipe == null) return;

            Picture = recipe.Picture ?? "";
            Name = recipe.Name;
            IsFavorite = recipe?.UserRecipes?.Any(ur => ur.UserId == userId) ?? false;

            if(!wantDetails)
            {
                // Divide by 100, because the ingredient are stored with the value for 100g proportions
                Calories = recipe!.RecipeIngredients.Sum(i => (i.Ingredient.Kcal * i.Quantity) / 100);
                return;
            }

            Description = recipe!.Description;
            Category = recipe.Category;
            Area = recipe.Area;
            YoutubeVideo = recipe.YoutubeVideo;
            RecipeIngredients = recipe.RecipeIngredients.Select(i => new ItemDTO(i));
        }
    }
}
