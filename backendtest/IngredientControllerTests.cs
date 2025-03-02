/// <summary>
/// This file contains the <c>IngredientControllerTests</c> class, which provides unit tests 
/// for the <see cref="IngredientController"/> class. These tests validate the functionality 
/// of retrieving ingredient data and handling authorization.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Controllers.Api;
using backend.Data;
using backend.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using backend.Models.Ingredients;

namespace backendtest
{
    /// <summary>
    /// The <c>IngredientControllerTests</c> class contains unit tests for the <see cref="IngredientController"/> API.
    /// It verifies that ingredient retrieval operations work correctly, including authorization handling.
    /// </summary>
    public class IngredientControllerTests : IClassFixture<ICareContextFixture>
    {
        private readonly ICareServerContext _context;
        private IngredientController _controller;

        /// <summary>
        /// Initializes a new instance of the <c>IngredientControllerTests</c> class.
        /// Sets up the database context and controller for testing.
        /// </summary>
        /// <param name="fixture">The test fixture that provides an in-memory database context.</param>
        public IngredientControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<IngredientController>.Instance;
            _controller = new(_context, logger);

            _context.Ingredients.RemoveRange(_context.Ingredients);
            _context.SaveChanges();
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns <see cref="UnauthorizedObjectResult"/> when no user ID is provided.
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
        /// Tests that <c>Get()</c> returns an empty list when there are no ingredients in the database.
        /// </summary>
        [Fact]
        public async Task Get_WhenNoIngredientsExist_ReturnsEmptyList()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<string>>>(result);

            var ingredients = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<string>;
            Assert.NotNull(ingredients);
            Assert.Empty(ingredients);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns a list of ingredient names when ingredients exist in the database.
        /// </summary>
        [Fact]
        public async Task Get_WhenIngredientsExist_ReturnsListOfIngredientNames()
        {
            _context.Ingredients.AddRange(
                new Ingredient { Name = "Sugar" },
                new Ingredient { Name = "Salt" }
            );

            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<string>>>(result);

            var ingredients = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<string>;
            Assert.NotNull(ingredients);
            Assert.Equal(_context.Ingredients.Count(), ingredients.Count);
            Assert.Contains("Sugar", ingredients);
            Assert.Contains("Salt", ingredients);
        }

        /// <summary>
        /// Tests that <c>Get(string)</c> returns <see cref="UnauthorizedObjectResult"/> when no user ID is provided.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Get("Sugar");

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Get(string)</c> returns <see cref="NotFoundObjectResult"/> when the requested ingredient does not exist.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenIngredientDoesNotExist_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("UnknownIngredient");

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Get(string)</c> returns the correct ingredient details when the ingredient exists in the database.
        /// </summary>
        [Fact]
        public async Task GetByName_WhenIngredientExists_ReturnsIngredientDetails()
        {
            var ingredient = new Ingredient
            {
                Name = "Sugar",
                Kcal = 387,
                KJ = 1630,
                Protein = 0,
                Carbohydrates = 99.8f,
                Lipids = 0,
                Fibers = 0,
                Category = "Sweeteners"
            };

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim("ValidUserId", _controller);

            var result = await _controller.Get("Sugar");

            Assert.NotNull(result);
            Assert.IsType<ActionResult<PublicIngredient>>(result);

            var ingredientDetails = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicIngredient;
            Assert.NotNull(ingredientDetails);
            Assert.Equal("Sugar", ingredientDetails.Name);
            Assert.Equal(387, ingredientDetails.Kcal);
            Assert.Equal(1630, ingredientDetails.KJ);
            Assert.Equal(0, ingredientDetails.Protein);
            Assert.Equal(Math.Round(99.8, 2), Math.Round(ingredientDetails.Carbohydrates, 2));
            Assert.Equal(0, ingredientDetails.Lipids);
            Assert.Equal(0, ingredientDetails.Fibers);
            Assert.Equal("Sweeteners", ingredientDetails.Category);
        }
    }
}