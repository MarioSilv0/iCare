/// <summary>
/// This file defines the <c>Recipe</c> class, which represents a recipe including its details, 
/// category, area of origin, and associations with ingredients and users.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Models.Ingredients;

namespace backend.Models.Recipes
{
    /// <summary>
    /// The <c>Recipe</c> class represents a recipe with its details such as name, 
    /// description, category, area of origin, and associated media. It also includes 
    /// relationships with ingredients and users.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Gets or sets the unique identifier for the recipe.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the URL of the recipe's picture.
        /// </summary>
        public string? Picture { get; set; }

        /// <summary>
        /// Gets or sets the name of the recipe.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of the recipe.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category of the recipe.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the area or region of origin for the recipe.
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// Gets or sets the URL of the recipe's YouTube video (if available).
        /// </summary>
        public string? YoutubeVideo { get; set; }

        /// <summary>
        /// Gets or sets the collection of ingredients associated with the recipe.
        /// </summary>
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        /// <summary>
        /// Gets or sets the collection of user interactions with this recipe, such as favorites.
        /// </summary>
        public ICollection<UserRecipe>? UserRecipes { get; set; }
    }
}
