using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Recipes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using backend.Models.Ingredients;

namespace backendtest
{
    public class RecipeControllerTests : IClassFixture<ICareContextFixture>, IAsyncLifetime
    {
        private readonly ICareServerContext _context;
        private RecipeController _controller;

        public RecipeControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<RecipeController>.Instance;
            _controller = new(_context, logger);
        }

        public async Task InitializeAsync()
        {
            _context.Recipes.RemoveRange(_context.Recipes);
            _context.Ingredients.RemoveRange(_context.Ingredients);
            _context.RecipeIngredients.RemoveRange(_context.RecipeIngredients);
            await _context.SaveChangesAsync();

            _context.Ingredients.AddRange(
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, Category = "Cereais e Derivados" },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 137, Category = "Carne" }
            );

            await _context.SaveChangesAsync();

            _context.Recipes.AddRange(
                new Recipe { Id = 1, Picture = "", Name = "Algo de Bom", Description = "Tu Consegues", Category = "Bom", Area = "Portugal", YoutubeVideo = "" },
                new Recipe { Id = 2, Picture = "", Name = "Algo de Mau", Description = "Boa Sorte", Category = "Mau", Area = "Bugs", YoutubeVideo = "" }
            );

            await _context.SaveChangesAsync();

            _context.RecipeIngredients.AddRange(
                new RecipeIngredient { RecipeId = 1, IngredientId = 1, Quantity = 200 }, // 124 * 2 = 248 kcal
                new RecipeIngredient { RecipeId = 1, IngredientId = 2, Quantity = 150 }, // 137 * 1.5 = 205.5 kcal
                new RecipeIngredient { RecipeId = 2, IngredientId = 3, Quantity = 100 }  // 137 * 1 = 137 kcal
            );

            await _context.SaveChangesAsync();

            _context.Users.Add(new User { Id = "ValidUserId", Email = "a@example.com" });
        }

        public Task DisposeAsync() => Task.CompletedTask;

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

        [Fact]
        public async Task GetByName_WhenRecipeDoesNotExist_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("NonExistingRecipe");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

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
            Assert.Equal("Tu Consegues", recipe.Description);
            Assert.Equal("Bom", recipe.Category);
            Assert.Equal("Portugal", recipe.Area);
            Assert.Equal("", recipe.YoutubeVideo);
            Assert.Equal(0, recipe.Calories);
            Assert.NotNull(recipe.RecipeIngredients);
            Assert.Equal(2, recipe.RecipeIngredients.Count());

            var ingredient1 = recipe.RecipeIngredients.FirstOrDefault(i => i.Name == "Arroz");
            var ingredient2 = recipe.RecipeIngredients.FirstOrDefault(i => i.Name == "Batata");

            Assert.NotNull(ingredient1);
            Assert.Equal(200, ingredient1.Quantity);

            Assert.NotNull(ingredient2);
            Assert.Equal(150, ingredient2.Quantity);
        }


        [Fact]
        public async Task GetByName_WhenNoRecipesExist_ReturnsNotFound()
        {
            _context.Recipes.RemoveRange(_context.Recipes);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("Algo de Bom");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}
