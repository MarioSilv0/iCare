/// <summary>
/// This file defines the <c>RecipeIngredient</c> class, which represents the relationship between 
/// recipes and ingredients. It is used to establish many-to-many associations between recipes and ingredients.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-03</date>

using backend.Models.Recipes;

namespace backend.Models.Ingredients
{
    /// <summary>
    /// The <c>RecipeIngredient</c> class represents the association between a recipe and an ingredient.
    /// It establishes a many-to-many relationship where multiple ingredients can belong to a recipe,
    /// and multiple recipes can contain the same ingredient.
    /// </summary>
    public class RecipeIngredient
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ingredient associated with this recipe.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Gets or sets the ingredient entity associated with this recipe.
        /// </summary>
        public Ingredient Ingredient { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the ingredient that the recipe needs.
        /// </summary>
        public float Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement for the ingredient quantity.
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the recipe associated with this ingredient.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Gets or sets the recipe entity associated with this ingredient.
        /// </summary>
        public Recipe Recipe { get; set; }
    }
}
