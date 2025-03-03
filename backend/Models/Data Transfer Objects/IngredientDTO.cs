
using backend.Models.Ingredients;

/// <summary>
/// This file defines the <c>PublicIngredient</c> class, which represents an ingredient's public data.
/// It includes properties such as nutritional information (calories, protein, carbohydrates, etc.) and category of the ingredient.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-03</date>
namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>PublicIngredient</c> class contains the public-facing details of an ingredient, including its nutritional values
    /// and category. This data is typically used for displaying ingredient information to users in an accessible format.
    /// </summary>
    public class IngredientDTO
    {
        /// <summary>
        /// Gets or sets the name of the ingredient.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the caloric content of the ingredient in kilocalories.
        /// </summary>
        public float Kcal { get; set; }

        /// <summary>
        /// Gets or sets the energy content of the ingredient in kilojoules.
        /// </summary>
        public float KJ { get; set; }

        /// <summary>
        /// Gets or sets the amount of protein in the ingredient (grams).
        /// </summary>
        public float Protein { get; set; }

        /// <summary>
        /// Gets or sets the amount of carbohydrates in the ingredient (grams).
        /// </summary>
        public float Carbohydrates { get; set; }

        /// <summary>
        /// Gets or sets the amount of lipids (fats) in the ingredient (grams).
        /// </summary>
        public float Lipids { get; set; }

        /// <summary>
        /// Gets or sets the amount of fibers in the ingredient (grams).
        /// </summary>
        public float Fibers { get; set; }

        /// <summary>
        /// Gets or sets the category to which the ingredient belongs.
        /// </summary>
        public string? Category { get; set; }

        public IngredientDTO(Ingredient ingredient) {
            Name = ingredient.Name;
            Kcal = ingredient.Kcal;
            KJ = ingredient.KJ;
            Protein = ingredient.Protein;
            Carbohydrates = ingredient.Carbohydrates;
            Lipids = ingredient.Lipids;
            Fibers = ingredient.Fibers;
            Category = ingredient.Category;
        }
    }
}
