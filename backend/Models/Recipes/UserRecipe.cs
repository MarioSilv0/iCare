/// <summary>
/// This file defines the <c>UserRecipe</c> class, which represents the relationship between 
/// users and their favorite recipes. It establishes a many-to-many association between 
/// users and recipes.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

namespace backend.Models.Recipes
{
    /// <summary>
    /// The <c>UserRecipe</c> class represents a user's favorite recipe, establishing 
    /// a many-to-many relationship between users and recipes.
    /// </summary>
    public class UserRecipe
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user who favorited the recipe.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user entity associated with this favorite recipe.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the recipe that the user favorited.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// Gets or sets the recipe entity associated with this user.
        /// </summary>
        public Recipe Recipe { get; set; }
    }
}
