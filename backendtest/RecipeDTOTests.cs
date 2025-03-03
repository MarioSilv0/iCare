using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using backend.Models.Recipes;
using Xunit;

namespace backendtest
{
    public class RecipeDTOTests
    {
        [Fact]
        public void Constructor_WhenRecipeIsNull_DoesNotThrow()
        {
            var dto = new RecipeDTO(null, true, "UserId");

            Assert.Null(dto.Name);
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
            Assert.Null(dto.Area);
            Assert.Null(dto.YoutubeVideo);
            Assert.Null(dto.RecipeIngredients);
            Assert.False(dto.IsFavorite);
            Assert.Equal(0, dto.Calories);
        }

        [Fact]
        public void Constructor_WhenWantDetailsIsFalse_CalculatesCaloriesCorrectly()
        {
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = new Ingredient { Name = "Arroz", Kcal = 124 }, Quantity = 200 }, // (124 * 200) / 100 = 248
                    new RecipeIngredient { Ingredient = new Ingredient { Name = "Batata", Kcal = 137 }, Quantity = 150 }  // (137 * 150) / 100 = 205.5
                }
            };
            float totalCalories = 453.5f;// 248 + 205.5

            var dto = new RecipeDTO(recipe, false, "UserId");

            Assert.Equal("Test Recipe", dto.Name);
            Assert.Equal(totalCalories, dto.Calories);
            Assert.False(dto.IsFavorite);
            Assert.Null(dto.Description);
            Assert.Null(dto.Category);
            Assert.Null(dto.Area);
            Assert.Null(dto.YoutubeVideo);
            Assert.Null(dto.RecipeIngredients);
        }

        [Fact]
        public void Constructor_WhenWantDetailsIsTrue_IncludesDetails()
        {
            var ingredient1 = new Ingredient { Name = "Arroz", Kcal = 124 };
            var ingredient2 = new Ingredient { Name = "Batata", Kcal = 137 };

        var recipe = new Recipe
            {
                Id = 1,
                Name = "Algo de Bom",
                Description = "Tu Consegues",
                Category = "Bom",
                Area = "Portugal",
                YoutubeVideo = "https://youtube.com",
                RecipeIngredients = new List<RecipeIngredient>
                {
                    new RecipeIngredient { Ingredient = ingredient1, Quantity = 200 },
                    new RecipeIngredient { Ingredient = ingredient2, Quantity = 150 },
                }
            };

            var dto = new RecipeDTO(recipe, true, "UserId");

            Assert.Equal("Algo de Bom", dto.Name);
            Assert.Equal(0, dto.Calories);
            Assert.False(dto.IsFavorite);
            Assert.Equal("Tu Consegues", dto.Description);
            Assert.Equal("Bom", dto.Category);
            Assert.Equal("Portugal", dto.Area);
            Assert.Equal("https://youtube.com", dto.YoutubeVideo);
            Assert.NotNull(dto.RecipeIngredients);
            Assert.Equal(2, dto.RecipeIngredients.Count());
            Assert.Contains(dto.RecipeIngredients, i => i.Name == ingredient1.Name);
            Assert.Contains(dto.RecipeIngredients, i => i.Name == ingredient2.Name);
        }

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

            var dto = new RecipeDTO(recipe, false, "UserId");

            Assert.True(dto.IsFavorite);
        }

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

            var dto = new RecipeDTO(recipe, false, "UserId");

            Assert.False(dto.IsFavorite);
        }
    }
}
