/// <summary>
/// This file defines the <c>RecipeDTO</c> class, which serves as a Data Transfer Object (DTO)
/// for transferring recipe data between the backend and frontend.
/// It includes relevant details such as the recipe's name, category, ingredients, calorie count, 
/// and favorite status for the authenticated user.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Models.Recipes;

namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// Represents a Data Transfer Object (DTO) for recipes, containing relevant details
    /// such as name, category, ingredients, and nutritional information.
    /// </summary>
    public class RecipeDTO
    {
        /// <summary>
        /// Gets or sets the URL of the recipe's picture.
        /// </summary>
        public string? Picture { get; set; }

        /// <summary>
        /// Gets or sets the name of the recipe.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the recipe.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the recipe (e.g., "Dessert", "Vegan").
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the geographical origin or area of the recipe.
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// Gets or sets the URL to a YouTube video for the recipe.
        /// </summary>
        public string? YoutubeVideo { get; set; }

        /// <summary>
        /// Gets or sets the list of ingredients used in the recipe.
        /// </summary>
        public IEnumerable<ItemDTO>? Ingredients { get; set; }

        /// <summary>
        /// Indicates whether the recipe is marked as a favorite by the authenticated user.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Gets or sets the total calorie count of the recipe.
        /// </summary>
        public float Calories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <c>RecipeDTO</c> class based on a given <c>Recipe</c> object.
        /// </summary>
        /// <param name="recipe">The recipe from which to create the DTO.</param>
        /// <param name="wantDetails">Indicates whether to include full recipe details.</param>
        /// <param name="userId">The ID of the user to check for favorite status.</param>
        public RecipeDTO(Recipe recipe, bool wantDetails, string userId)
        {
            if (recipe == null) return;

            Picture = recipe.Picture ?? "";
            Name = recipe.Name;
            Category = recipe.Category;
            IsFavorite = recipe?.UserRecipes?.Any(ur => ur.UserId == userId) ?? false;
            Ingredients = recipe!.RecipeIngredients.Select(i => new ItemDTO(i));

            if (!wantDetails)
            {
                // Divide by 100, because the ingredient are stored with the value for 100g proportions
                Calories = recipe!.RecipeIngredients.Sum(i => (i.Ingredient.Kcal * i.Quantity) / 100);
                return;
            }

            Description = recipe!.Description;
            Area = recipe.Area;
            YoutubeVideo = recipe.YoutubeVideo;
        }
    }
}
