/// <summary>
/// Data Transfer Object (DTO) for representing the association between recipes and ingredients.
/// This DTO is used to facilitate data transfer without exposing database entities directly.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
using backend.Models.Ingredients;

namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// The <c>RecipeIngredientDTO</c> class represents the association between a recipe and an ingredient.
    /// It is used to transfer data between the client and server, avoiding direct entity exposure.
    /// </summary>
    public class RecipeIngredientDTO
    {
        /// <summary>
        /// Gets or sets the name of the ingredient.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the measure of the ingredient.
        /// </summary>
        public string? Measure { get; set; }

        /// <summary>
        /// Gets or sets the grams of the ingredient.
        /// </summary>
        public float? Grams { get; set; }

        /// <summary>
        /// Default constructor for the <c>RecipeIngredientDTO</c> class.
        /// </summary>
        public RecipeIngredientDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>RecipeIngredientDTO</c> class using an <see cref="RecipeIngredient"/> object.
        /// This constructor extracts relevant details for data transfer.
        /// </summary>
        /// <param name="recipeIngredient">The <see cref="RecipeIngredient"/> entity containing relationship details.</param>
        public RecipeIngredientDTO(RecipeIngredient recipeIngredient)
        {
            Name = recipeIngredient.Ingredient?.Name;
            Measure = recipeIngredient.Measure;
            Grams = recipeIngredient.Grams;
        }

    }
}
