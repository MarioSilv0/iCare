/// <summary>
/// This file defines the <c>UserIngredient</c> class, which represents an ingredient 
/// associated with a user, including its quantity, unit, and user reference.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

namespace backend.Models.Ingredients
{
    /// <summary>
    /// The <c>UserIngredient</c> class represents an ingredient that belongs to a user, 
    /// including its quantity, unit of measurement, and a reference to the user.
    /// This class establishes a many-to-many relationship between users and ingredients.
    /// </summary>
    public class UserIngredient
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ingredient associated with the user.
        /// </summary>
        public int IngredientId { get; set; }

        /// <summary>
        /// Gets or sets the ingredient entity associated with the user.
        /// </summary>
        public Ingredient Ingredient { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the ingredient owned by the user.
        /// </summary>
        public float Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement for the ingredient quantity.
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who owns this ingredient.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user entity associated with this ingredient.
        /// </summary>
        public User User { get; set; }
    }
}
