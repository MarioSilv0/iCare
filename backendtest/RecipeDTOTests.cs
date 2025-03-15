/// <summary>
/// This file contains unit tests for the <c>RecipeDTO</c> class, ensuring correct behavior
/// when creating recipe DTOs from recipe models.
/// The tests cover scenarios such as calorie calculation, favorite status, and property assignments.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using backend.Models.Recipes;

namespace backendtest
{
    /// <summary>
    /// This class contains unit tests for the <c>RecipeDTO</c> constructor and its behavior.
    /// It verifies how the <c>RecipeDTO</c> handles different scenarios, including null values, 
    /// calorie calculations, favorite status, and whether detailed information is included.
    /// </summary>
    public class RecipeDTOTests
    {
        /// <summary>
        /// Tests if the constructor handles a null <c>Recipe</c> object gracefully.
        /// It ensures that all properties are correctly initialized to null or default values.
        /// </summary>
        [Fact]
        public void Constructor_WhenRecipeIsNull_DoesNotThrow()
        {
            var dto = new RecipeDTO(null, "UserId", false);

            Assert.Null(dto.Name);
            Assert.Null(dto.Category);
            Assert.Null(dto.Area);
            Assert.Null(dto.UrlVideo);
            Assert.Null(dto.Instructions);
            Assert.Null(dto.Ingredients);
            Assert.False(dto.IsFavorite);
            Assert.Equal(0, dto.Calories);
        }

        /// <summary>
        /// Tests if the constructor correctly calculates the total calories when 
        /// <paramref name="wantDetails"/> is set to false.
        /// </summary>
        [Fact]
        public void Constructor_WhenWantDetailsIsFalse_CalculatesCaloriesCorrectly()
        {
            float value = 123, expectedValue = 0;
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                Category = "Test Category",
                Area = "Test Area",
                Calories = value,
                Proteins = value,
                Lipids = value,
                Fibers = value,
                Carbohydrates = value,
                UrlVideo = "Test Url",
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = new Ingredient { Name = "Arroz", Kcal = 124 }, Measure = "medida", Grams = 200 }, // (124 * 200) / 100 = 248
                    new RecipeIngredient { Ingredient = new Ingredient { Name = "Batata", Kcal = 137 }, Measure = "medida", Grams = 150 }  // (137 * 150) / 100 = 205.5
                }
            };

            var dto = new RecipeDTO(recipe, "UserId", false);

            Assert.Equal("Test Recipe", dto.Name);
            Assert.Equal("Test Category", dto.Category);
            Assert.Equal("Test Area", dto.Area);
            Assert.Equal(2, dto.Ingredients!.Count());
            Assert.Equal(expectedValue, dto.Calories);
            Assert.Equal(expectedValue, dto.Proteins);
            Assert.Equal(expectedValue, dto.Carbohydrates);
            Assert.Equal(expectedValue, dto.Lipids);
            Assert.Equal(expectedValue, dto.Fibers);
            Assert.False(dto.IsFavorite);
            Assert.Null(dto.UrlVideo);
            Assert.Null(dto.Instructions);
        }

        /// <summary>
        /// Tests if the constructor includes all recipe details when 
        /// <paramref name="wantDetails"/> is set to true.
        /// </summary>
        [Fact]
        public void Constructor_WhenWantDetailsIsTrue_IncludesDetails()
        {
            var ingredient1 = new Ingredient { Name = "Arroz", Kcal = 124 };
            var ingredient2 = new Ingredient { Name = "Batata", Kcal = 137 };

            float expectedValue = 123;

            var recipe = new Recipe
            {
                Id = 1,
                Name = "Algo de Bom",
                Category = "Bom",
                Area = "Portugal",
                UrlVideo = "https://youtube.com",
                Instructions = "Tu Consegues",
                Calories = expectedValue,
                Proteins = expectedValue,
                Lipids = expectedValue,
                Fibers = expectedValue,
                Carbohydrates = expectedValue,
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = ingredient1, Measure = "medida", Grams = 200 },
                    new RecipeIngredient { Ingredient = ingredient2, Measure = "medida", Grams = 150 },
                }
            };

            var dto = new RecipeDTO(recipe, "UserId", true);

            Assert.Equal("Algo de Bom", dto.Name);
            Assert.False(dto.IsFavorite);
            Assert.Equal("Tu Consegues", dto.Instructions);
            Assert.Equal("Bom", dto.Category);
            Assert.Equal("Portugal", dto.Area);
            Assert.Equal("https://youtube.com", dto.UrlVideo);
            Assert.NotNull(dto.Ingredients);
            Assert.Equal(2, dto.Ingredients.Count());
            Assert.Contains(dto.Ingredients, i => i.Name == ingredient1.Name);
            Assert.Contains(dto.Ingredients, i => i.Name == ingredient2.Name);
            Assert.Equal(2, dto.Ingredients!.Count());
            Assert.Equal(expectedValue, dto.Calories);
            Assert.Equal(expectedValue, dto.Proteins);
            Assert.Equal(expectedValue, dto.Carbohydrates);
            Assert.Equal(expectedValue, dto.Lipids);
            Assert.Equal(expectedValue, dto.Fibers);
        }

        /// <summary>
        /// Tests if the constructor correctly identifies a recipe as a favorite
        /// when the authenticated user's ID matches one of the <c>UserRecipe</c> entries.
        /// </summary>
        [Fact]
        public void Constructor_WhenUserIsFavorite_SetsIsFavoriteToTrue()
        {
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                UserRecipes = new List<UserRecipe>
                {
                    new UserRecipe { UserId = "UserId" }
                }
            };

            var dto = new RecipeDTO(recipe, "UserId", false);

            Assert.True(dto.IsFavorite);
        }

        /// <summary>
        /// Tests if the constructor correctly identifies a recipe as NOT a favorite
        /// when the authenticated user's ID does not match any <c>UserRecipe</c> entries.
        /// </summary>
        [Fact]
        public void Constructor_WhenUserIsNotFavorite_SetsIsFavoriteToFalse()
        {
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                UserRecipes = new List<UserRecipe>
                {
                    new UserRecipe { UserId = "AnotherUserId" }
                }
            };

            var dto = new RecipeDTO(recipe, "UserId", false);

            Assert.False(dto.IsFavorite);
        }
    }
}
