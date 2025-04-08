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
    /// such as the name, category, area, instructions, ingredients, and nutritional information.
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
        public string? UrlVideo { get; set; }

        /// <summary>
        /// Gets or sets the list of instructions for preparing the recipe.
        /// </summary>
        public string? Instructions { get; set; }

        /// <summary>
        /// Gets or sets the list of ingredients used in the recipe.
        /// </summary>
        public IEnumerable<RecipeIngredientDTO>? Ingredients { get; set; }

        /// <summary>
        /// Gets or sets the total calorie count of the recipe.
        /// </summary>
        public float Calories { get; set; }

        /// <summary>
        /// Gets or sets the total proteins count of the recipe.
        /// </summary>
        public float Proteins { get; set; }

        /// <summary>
        /// Gets or sets the total carbohydrates count of the recipe.
        /// </summary>
        public float Carbohydrates { get; set; }

        /// <summary>
        /// Gets or sets the total lipids (fats) count of the recipe.
        /// </summary>
        public float Lipids { get; set; }

        /// <summary>
        /// Gets or sets the total fibers count of the recipe.
        /// </summary>
        public float Fibers { get; set; }

        /// <summary>
        /// Indicates whether the recipe is marked as a favorite by the authenticated user.
        /// </summary>
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Default constructor for the <c>RecipeDTO</c> class.
        /// </summary>
        public RecipeDTO() { }

        /// <summary>
        /// Constructs a <c>RecipeDTO</c> from a <see cref="Recipe"/> entity.
        /// </summary>
        /// <param name="recipe">The <see cref="Recipe"/> object containing the recipe data.</param>
        /// <param name="userId">The ID of the authenticated user to check for the favorite status.</param>
        /// <param name="wantDetails">Determines whether to include detailed recipe information (e.g., calories, instructions).</param>
        public RecipeDTO(Recipe recipe, string userId, bool wantDetails)
        {
            // If the recipe is null, return without setting any properties
            if (recipe == null) return;

            // Set the basic properties
            Picture = recipe.Picture ?? "";  // If Picture is null, default to an empty string
            Name = recipe.Name;
            Category = recipe.Category;
            Area = recipe.Area;

            // Map the recipe ingredients to their DTO equivalent
            Ingredients = recipe.RecipeIngredients.Select(i => new RecipeIngredientDTO(i));

            // Check if the authenticated user has marked this recipe as a favorite
            IsFavorite = recipe?.UserRecipes?.Any(ur => ur.UserId == userId) ?? false;

            // If detailed information is requested, populate the additional properties
            if (!wantDetails) return;

            // Set detailed recipe information
            UrlVideo = recipe!.UrlVideo;
            Instructions = recipe.Instructions;
            Calories = recipe.Calories;
            Proteins = recipe.Proteins;
            Carbohydrates = recipe.Carbohydrates;
            Lipids = recipe.Lipids;
            Fibers = recipe.Fibers;
        }
    }
}
