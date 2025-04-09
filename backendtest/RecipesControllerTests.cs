/// <summary>
/// This file contains unit tests for the <c>RecipeController</c> class.
/// The tests ensure that API endpoints for retrieving recipe details function correctly, 
/// including authorization checks, retrieving recipes by name, and verifying calorie calculations.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Recipes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using backend.Models.Ingredients;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace backendtest
{
    /// <summary>
    /// Unit tests for the <c>RecipeController</c> class.
    /// These tests verify the behavior of the API endpoints for retrieving and managing recipes.
    /// </summary>
    public class RecipeControllerTests : IClassFixture<ICareContextFixture>, IAsyncLifetime
    {
        private readonly ICareServerContext _context;
        private RecipeController _controller;

        /// <summary>
        /// Initializes the test class by setting up a fresh database context.
        /// </summary>
        /// <param name="fixture">Provides the in-memory database context.</param>
        public RecipeControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<RecipeController>.Instance;
            _controller = new(_context, logger);
        }

        /// <summary>
        /// Clears the database before running each test and adds test data.
        /// </summary>
        public async Task InitializeAsync()
        {
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.Ingredients.RemoveRange(_context.Ingredients);
            _context.RecipeIngredients.RemoveRange(_context.RecipeIngredients);
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();

            _context.Ingredients.AddRange(
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, Category = "Cereais e Derivados", Protein = 15, Carbohydrates = 30, Fibers = 20, Lipids = 5 },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 137, Category = "Carne", Protein = 40, Carbohydrates = 20, Fibers = 10, Lipids = 10 }
            );

            _context.Recipes.AddRange(
                new Recipe { Id = 1, Picture = "", Name = "Algo de Bom", Category = "Bom", Area = "Portugal", UrlVideo = "", Instructions = "Tu Consegues" },
                new Recipe { Id = 2, Picture = "", Name = "Algo de Mau", Category = "Mau", Area = "Bugs", UrlVideo = "", Instructions = "Boa Sorte", Calories = 123, Proteins = 321, Carbohydrates = 231, Lipids = 213, Fibers = 312 }
            );

            _context.RecipeIngredients.AddRange(
                new RecipeIngredient { RecipeId = 1, IngredientId = 1, Measure = "medida", Grams = 200 },
                new RecipeIngredient { RecipeId = 1, IngredientId = 2, Measure = "medida", Grams = 150 },
                new RecipeIngredient { RecipeId = 2, IngredientId = 3, Measure = "medida", Grams = 100 }
            );

            _context.Users.Add(new User { Id = "ValidUserId", Email = "a@example.com" });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cleans up any resources after the tests have completed.
        /// </summary>
        public Task DisposeAsync() => Task.CompletedTask;

        /// <summary>
        /// Tests if the API returns unauthorized when the user ID is not provided.
        /// </summary>
        [Fact]
        public async Task Get_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Get();

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests if the API correctly retrieves a list of recipes with accurate calorie calculations.
        /// </summary>
        [Fact]
        public async Task Get_WhenRecipesExist_ReturnsListOfRecipesWithCorrectCalories()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<RecipeDTO>>>(result);

            var recipes = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<RecipeDTO>;
            Assert.NotNull(recipes);
            Assert.Equal(2, recipes.Count);

            var recipe1 = recipes.FirstOrDefault(r => r.Name == "Algo de Bom");
            var recipe2 = recipes.FirstOrDefault(r => r.Name == "Algo de Mau");
            
            Assert.NotNull(recipe1);
            Assert.NotNull(recipe2);
            Assert.Equal(0, recipe1.Calories);
            Assert.Equal(0, recipe1.Proteins);
            Assert.Equal(0, recipe1.Carbohydrates);
            Assert.Equal(0, recipe1.Lipids);
            Assert.Equal(0, recipe1.Fibers);
            Assert.Equal(0, recipe2.Calories);
            Assert.Equal(0, recipe2.Proteins);
            Assert.Equal(0, recipe2.Carbohydrates);
            Assert.Equal(0, recipe2.Lipids);
            Assert.Equal(0, recipe2.Fibers);
        }

        /// <summary>
        /// Tests if the API returns an empty list when there are no recipes available.
        /// </summary>
        [Fact]
        public async Task Get_WhenNoRecipesExist_ReturnsEmptyList()
        {
            _context.Recipes.RemoveRange(_context.Recipes);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<RecipeDTO>>>(result);

            var recipes = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<RecipeDTO>;
            Assert.NotNull(recipes);
            Assert.Empty(recipes);
        }

        /// <summary>
        /// Tests if the API returns unauthorized when retrieving a recipe by name without a user ID.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Get("Algo de Bom");

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests if the API returns not found when the requested recipe does not exist.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenRecipeDoesNotExist_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("NonExistingRecipe");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests if the API retrieves a recipe with detailed information.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenRecipeExists_ReturnsRecipeDTOWithDetails()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("Algo de Mau");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<RecipeDTO>>(result);

            var recipe = Assert.IsType<OkObjectResult>(result.Result)?.Value as RecipeDTO;
            Assert.NotNull(recipe);
            Assert.Equal("Algo de Mau", recipe.Name);
            Assert.Equal("Boa Sorte", recipe.Instructions);
            Assert.Equal("Mau", recipe.Category);
            Assert.Equal("Bugs", recipe.Area);
            Assert.Equal("", recipe.UrlVideo);
            Assert.Equal(123, recipe.Calories);
            Assert.Equal(321, recipe.Proteins);
            Assert.Equal(231, recipe.Carbohydrates);
            Assert.Equal(213, recipe.Lipids);
            Assert.Equal(312, recipe.Fibers);
            Assert.NotNull(recipe.Ingredients);
            Assert.Single(recipe.Ingredients);

            var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Name == "Carne");

            Assert.NotNull(ingredient);
            Assert.Equal(100, ingredient.Grams);
            Assert.Equal("medida", ingredient.Measure);
        }

        /// <summary>
        /// Tests if the API returns a bad request when the recipes list is null.
        /// </summary>
        [Fact]
        public async Task UpdateRecipes_WhenRecipesListIsNull_ReturnsBadRequest()
        {
            var result = await _controller.UpdateRecipes(null);
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No recipes provided.", response.Value);
        }

        /// <summary>
        /// Tests if the API returns a bad request when the recipes list is empty.
        /// </summary>
        [Fact]
        public async Task UpdateRecipes_WhenRecipesListIsEmpty_ReturnsBadRequest()
        {
            var result = await _controller.UpdateRecipes(new List<RecipeDTO>());
            var response = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No recipes provided.", response.Value);
        }

        /// <summary>
        /// Tests if the API correctly adds a new recipe when provided with a valid recipe.
        /// </summary>
        [Fact]
        public async Task UpdateRecipes_WhenNewRecipeIsAdded_AddsRecipeWithCalculatedValues()
        {
            var recipes = new List<RecipeDTO>
            {
                new RecipeDTO
                {
                    Name = "Arroz com Carne",
                    Area = "Portugal",
                    Category = "Main",
                    Picture = "pic.jpg",
                    UrlVideo = "youtube.com",
                    Instructions = "Cook everything.",
                    Ingredients = new List<RecipeIngredientDTO>
                    {
                        new RecipeIngredientDTO { Name = "Arroz", Measure = "measure", Grams = 100 },
                        new RecipeIngredientDTO { Name = "Carne", Measure = "measure", Grams = 200 }
                    }
                }
            };

            var result = await _controller.UpdateRecipes(recipes);
            Assert.IsType<OkObjectResult>(result);

            var insertedRecipe = await _context.Recipes.Include(r => r.RecipeIngredients)
                                                       .ThenInclude(ri => ri.Ingredient)
                                                       .FirstOrDefaultAsync(r => r.Name == "Arroz com Carne");

            Assert.NotNull(insertedRecipe);
            Assert.Equal("Arroz com Carne", insertedRecipe.Name);
            Assert.Equal("Portugal", insertedRecipe.Area);
            Assert.Equal("Main", insertedRecipe.Category);
            Assert.Equal("pic.jpg", insertedRecipe.Picture);
            Assert.Equal("youtube.com", insertedRecipe.UrlVideo);
            Assert.Equal("Cook everything.", insertedRecipe.Instructions); 
            Assert.Equal(398, insertedRecipe.Calories); // (100g * 124kcal / 100g) + (200g * 137kcal / 100) = 
            Assert.Equal(95, insertedRecipe.Proteins);  
            Assert.Equal(70, insertedRecipe.Carbohydrates);
            Assert.Equal(40, insertedRecipe.Fibers);
            Assert.Equal(25, insertedRecipe.Lipids);
            Assert.Equal(2, insertedRecipe.RecipeIngredients.Count);
        }

        /// <summary>
        /// Tests if the API correctly updates an existing recipe's details.
        /// </summary>
        [Fact]
        public async Task UpdateRecipes_WhenRecipeExists_UpdatesRecipeDetails()
        {
            // Add initial recipe
            var recipe = new Recipe
            {
                Name = "Simple",
                Area = "OldArea",
                Category = "OldCat",
                Picture = "oldpic",
                UrlVideo = "oldurl",
                Instructions = "Old",
                RecipeIngredients = new List<RecipeIngredient>(),
                Calories = 0,
                Proteins = 0,
                Carbohydrates = 0,
                Lipids = 0,
                Fibers = 0
            };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            var updatedDTO = new RecipeDTO
            {
                Name = "Simple",
                Area = "NewArea",
                Category = "NewCat",
                Picture = "newpic",
                UrlVideo = "newurl",
                Instructions = "NewInstructions",
                Calories = 123,
                Proteins = 12,
                Carbohydrates = 22,
                Lipids = 11,
                Fibers = 9,
                Ingredients = new List<RecipeIngredientDTO>
                {
                    new RecipeIngredientDTO { Name = "Arroz", Measure = "100g", Grams = 100 }
                }
            };

            var result = await _controller.UpdateRecipes(new List<RecipeDTO> { updatedDTO });
            Assert.IsType<OkObjectResult>(result);

            var updated = await _context.Recipes.Include(r => r.RecipeIngredients).FirstOrDefaultAsync(r => r.Name == "Simple");
            Assert.NotNull(updated);
            Assert.Equal("NewArea", updated.Area);
            Assert.Equal("NewCat", updated.Category);
            Assert.Equal("newpic", updated.Picture);
            Assert.Equal("newurl", updated.UrlVideo);
            Assert.Equal("NewInstructions", updated.Instructions);
            Assert.Equal(123, updated.Calories);
            Assert.Equal(12, updated.Proteins);
            Assert.Equal(22, updated.Carbohydrates);
            Assert.Equal(11, updated.Lipids);
            Assert.Equal(9, updated.Fibers);
            Assert.Single(updated.RecipeIngredients);
        }
    }
}
