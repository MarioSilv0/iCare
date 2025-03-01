namespace backend.Models.Recipes
{
    public class UserRecipe // Favorite Recipes of the user
    {
        /// <value>
        /// Property <c>UserId</c> represents the unique identifier of the user.
        /// </value>
        public string UserId { get; set; }

        /// <value>
        /// Property <c>User</c> represents the user associated with the Recipe.
        /// </value>
        public User User { get; set; }

        /// <value>
        /// Property <c>RecipeId</c> represents the unique identifier of the recipe.
        /// </value>
        public int RecipeId { get; set; }

        /// <value>
        /// Property <c>Recipe</c> represents the recipe associated with the user.
        /// </value>
        public Recipe Recipe { get; set; }
    }
}
