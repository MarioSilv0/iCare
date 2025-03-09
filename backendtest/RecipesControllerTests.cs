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
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, Category = "Cereais e Derivados" },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 137, Category = "Carne" }
            );

            _context.Recipes.AddRange(
                new Recipe { Id = 1, Picture = "", Name = "Algo de Bom", Category = "Bom", Area = "Portugal", UrlVideo = "", Instructions = "Tu Consegues" },
                new Recipe { Id = 2, Picture = "", Name = "Algo de Mau", Category = "Mau", Area = "Bugs", UrlVideo = "", Instructions = "Boa Sorte" }
            );

            _context.RecipeIngredients.AddRange(
                new RecipeIngredient { RecipeId = 1, IngredientId = 1, Measure = "medida", Grams = 200 }, // 124 * 2 = 248 kcal
                new RecipeIngredient { RecipeId = 1, IngredientId = 2, Measure = "medida", Grams = 150 }, // 137 * 1.5 = 205.5 kcal
                new RecipeIngredient { RecipeId = 2, IngredientId = 3, Measure = "medida", Grams = 100 }  // 137 * 1 = 137 kcal
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
            float calories1 = 453.5f, calories2 = 137f;
            
            Assert.NotNull(recipe1);
            Assert.NotNull(recipe2);
            Assert.Equal(calories1, recipe1.Calories);
            Assert.Equal(calories2, recipe2.Calories);
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

            var result = await _controller.Get("Algo de Bom");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<RecipeDTO>>(result);

            var recipe = Assert.IsType<OkObjectResult>(result.Result)?.Value as RecipeDTO;
            Assert.NotNull(recipe);
            Assert.Equal("Algo de Bom", recipe.Name);
            Assert.Equal("Tu Consegues", recipe.Instructions);
            Assert.Equal("Bom", recipe.Category);
            Assert.Equal("Portugal", recipe.Area);
            Assert.Equal("", recipe.UrlVideo);
            Assert.Equal(0, recipe.Calories);
            Assert.NotNull(recipe.Ingredients);
            Assert.Equal(2, recipe.Ingredients.Count());

            var ingredient1 = recipe.Ingredients.FirstOrDefault(i => i.Name == "Arroz");
            var ingredient2 = recipe.Ingredients.FirstOrDefault(i => i.Name == "Batata");

            Assert.NotNull(ingredient1);
            Assert.Equal(200, ingredient1.Grams);

            Assert.NotNull(ingredient2);
            Assert.Equal(150, ingredient2.Grams);
        }
    }
}
