/// <summary>
/// This file defines the <c>Ingredient</c> class, which represents an ingredient with its nutritional values,
/// category, and relationships to users and recipes.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

namespace backend.Models.Ingredients
{
    /// <summary>
    /// The <c>Ingredient</c> class represents a food ingredient, including its nutritional values
    /// and category. It also defines relationships with user and recipe ingredients.
    /// </summary>
    public class Ingredient
    {
        /// <summary>
        /// Gets or sets the unique identifier for the ingredient.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the ingredient.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the caloric content of the ingredient in kilocalories (kcal).
        /// </summary>
        public float Kcal {  get; set; }

        /// <summary>
        /// Gets or sets the energy content of the ingredient in kilojoules (kJ).
        /// </summary>
        public float KJ { get; set; }

        /// <summary>
        /// Gets or sets the protein content of the ingredient in grams.
        /// </summary>
        public float Protein { get; set; }

        /// <summary>
        /// Gets or sets the carbohydrate content of the ingredient in grams.
        /// </summary>
        public float Carbohydrates { get; set; }

        /// <summary>
        /// Gets or sets the lipid (fat) content of the ingredient in grams.
        /// </summary>
        public float Lipids { get; set; }

        /// <summary>
        /// Gets or sets the fiber content of the ingredient in grams.
        /// </summary>
        public float Fibers { get; set; }

        /// <summary>
        /// Gets or sets the category of the ingredient.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the collection of user-specific ingredient entries related to this ingredient.
        /// </summary>
        public ICollection<UserIngredient>? UserIngredients { get; set; }

        /// <summary>
        /// Gets or sets the collection of recipe ingredient entries related to this ingredient.
        /// </summary>
        public ICollection<RecipeIngredient>? IngredientRecipes { get; set; }
    }
}
