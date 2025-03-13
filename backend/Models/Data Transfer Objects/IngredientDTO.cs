/// <summary>
/// This file defines the <c>IngredientDTO</c> class, which serves as a Data Transfer Object (DTO)
/// for representing an ingredient's public data.
/// It includes properties such as nutritional information (calories, protein, carbohydrates, fats, fibers)
/// and the category to which the ingredient belongs.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Models.Ingredients;

namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>IngredientDTO</c> class provides a simplified representation of an ingredient, including
    /// its nutritional values and category. This class is used for data transfer purposes between 
    /// the backend and frontend, ensuring structured and efficient communication.
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

        /// <summary>
        /// Default constructor for the <c>IngredientDTO</c> class.
        /// </summary>
        public IngredientDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>IngredientDTO</c> class using an <see cref="Ingredient"/> object.
        /// This constructor extracts relevant ingredient details for data transfer.
        /// </summary>
        /// <param name="ingredient">The <see cref="Ingredient"/> entity containing ingredient details.</param>
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
