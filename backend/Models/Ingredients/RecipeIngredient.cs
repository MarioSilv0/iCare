using backend.Models.Recipes;

namespace backend.Models.Ingredients
{
    public class RecipeIngredient
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ingredient associated with this recipe.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Gets or sets the ingredient associated with this recipe.
        /// </summary>
        public Ingredient Ingredient { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the recipe associated with this ingredient.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Gets or sets the recipe associated with this ingredient.
        /// </summary>
        public Recipe Recipe { get; set; }
    }
}
