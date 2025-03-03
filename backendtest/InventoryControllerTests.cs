/// <summary>
/// This file contains the <c>InventoryControllerTests</c> class, which provides unit tests 
/// for the <see cref="InventoryController"/>. It validates the behavior of inventory-related 
/// API endpoints such as retrieving, updating, and deleting user ingredients.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;

namespace backendtest
{
    /// <summary>
    /// The <c>InventoryControllerTests</c> class contains unit tests for the <see cref="InventoryController"/>.
    /// It verifies the behavior of API methods responsible for managing user inventory, 
    /// ensuring correct handling of valid and invalid requests.
    /// </summary>
    public class InventoryControllerTests : IClassFixture<ICareContextFixture>
    {
        private readonly ICareServerContext _context;
        private InventoryController controller;

        /// <summary>
        /// Initializes a new instance of the <c>InventoryControllerTests</c> class.
        /// Sets up a test database and an instance of the InventoryController for testing.
        /// </summary>
        /// <param name="fixture">The test fixture providing a shared in-memory database context.</param>
        public InventoryControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<InventoryController>.Instance;

            controller = new(_context, logger);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns <see cref="UnauthorizedObjectResult"/> 
        /// when the user ID is missing from the authentication token.
        /// </summary>
        [Fact]
        public async Task Get_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Get();

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns an empty list when the provided user ID does not exist.
        /// </summary>
        [Fact]
        public async Task Get_WhenIdIsInvalid_ReturnsEmptyList()
        {
            Authenticate.SetUserIdClaim("InvalidId", controller);

            var result = await controller.Get();
            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns a list of ingredients when a valid user ID is provided.
        /// </summary>
        [Fact]
        public async Task Get_WhenIdIsValid_ReturnsList()
        {
            User user = new User
            {
                Id = "Id 4",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 10, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 11, Name = "Bar" };

            UserIngredient item1 = new UserIngredient { IngredientId = ingredient1.Id, Quantity = 1, Unit = "kg", UserId = user.Id };
            UserIngredient item2 = new UserIngredient { IngredientId = ingredient2.Id, Quantity = 5, Unit = "ml", UserId = user.Id };

            user.UserIngredients.Add(item1);
            user.UserIngredients.Add(item2);

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var ingredients = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(ingredients);
            Assert.Equal(user.UserIngredients.Count, ingredients.Count);
            Assert.Contains(ingredients, i => i.Name == ingredient1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(ingredients, i => i.Name == ingredient2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns an empty list of ingredients when a valid user ID is provided and the user doesn't have ingredients associated.
        /// </summary>
        [Fact]
        public async Task Get_WhenUserHasNoItems_ReturnsEmptyList()
        {
            User user = new User
            {
                Id = "Id 5",
                UserIngredients = new List<UserIngredient>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that <c>Update()</c> returns <see cref="UnauthorizedObjectResult"/> 
        /// when the user ID is missing from the authentication token.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Update(new List<ItemDTO>());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Update()</c> correctly processes updates when a user has a null list of ingredients.
        /// It ensures that new ingredients are added properly.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasNullListOfIngredients_ReturnsList()
        {
            User user = new User
            {
                Id = "Id 6",
                UserIngredients = null
            };

            Ingredient ingredient1 = new Ingredient { Id = 12, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 13, Name = "Bar" };

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            ItemDTO item1 = new ItemDTO() { Name = ingredient1.Name, Quantity = 1, Unit = "kg" };
            ItemDTO item2 = new ItemDTO() { Name = ingredient2.Name, Quantity = 1, Unit = "kg" };

            List<ItemDTO> newItems = new () { item1, item2};

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == ingredient2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        /// <summary>
        /// Tests that <c>Update()</c> returns an empty list when a user tries to update ingredients 
        /// that do not exist in the database.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndTheIngredientsDontExist_ReturnsEmptyList()
        {
            User user = new User
            {
                Id = "Id 7",
                UserIngredients = new List<UserIngredient>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            Ingredient ingredient1 = new Ingredient { Id = 14, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 15, Name = "Bar" };

            ItemDTO item1 = new ItemDTO() { Name = ingredient1.Name, Quantity = 1, Unit = "kg" };
            ItemDTO item2 = new ItemDTO() { Name = ingredient2.Name, Quantity = 1, Unit = "kg" };

            List<ItemDTO> newItems = new() { item1, item2 };

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that <c>Update()</c> correctly updates a user’s ingredient list when they already have ingredients.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasListOfIngredients_ReturnsList()
        {

            User user = new User
            {
                Id = "Id 8",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 16, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 17, Name = "Bar" };

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            ItemDTO item1 = new ItemDTO() { Name = ingredient1.Name, Quantity = 1, Unit = "kg" };
            ItemDTO item2 = new ItemDTO() { Name = ingredient2.Name, Quantity = 1, Unit = "kg" };

            List<ItemDTO> newItems = new() { item1, item2 };

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == ingredient2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        /// <summary>
        /// Tests that <c>Update()</c> returns an empty list when a user provides an empty list of ingredients.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndUserAddsEmprtyListOfIngredients_ReturnsEmptyList()
        {

            User user = new User
            {
                Id = "Id 9",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 18, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 19, Name = "Bar" };

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            List<ItemDTO> newItems = new();

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that <c>Update()</c> removes duplicate ingredients and keeps only the last occurrence.
        /// Ensures that repeated ingredient entries are consolidated correctly.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndUserAddsRepeatedIngredients_ReturnsListWithoutRepetitions()
        {

            User user = new User
            {
                Id = "Id 10",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 20, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 21, Name = "Bar" };

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            ItemDTO item1 = new ItemDTO() { Name = ingredient1.Name, Quantity = 1, Unit = "kg" };
            ItemDTO item2 = new ItemDTO() { Name = ingredient2.Name, Quantity = 1, Unit = "kg" };
            ItemDTO item3 = new ItemDTO() { Name = ingredient1.Name, Quantity = 2, Unit = "g" };

            List<ItemDTO> newItems = new() { item1, item2, item3 };

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == item3.Quantity && i.Unit == item3.Unit);
            Assert.Contains(items, i => i.Name == ingredient2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        /// <summary>
        /// Tests that <c>Update()</c> correctly updates an existing ingredient's quantity and unit.
        /// Ensures that modifications to a user's existing ingredient list are applied.
        /// </summary>
        [Fact]
        public async Task Update_WhenIdIsValidAndUserEditsIngredient_ReturnsList()
        {

            User user = new User
            {
                Id = "Id 11",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 22, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 23, Name = "Bar" };

            user.UserIngredients.Add(new UserIngredient { IngredientId = ingredient1.Id, Quantity = 1, Unit = "kg", UserId = user.Id });

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            ItemDTO item1 = new ItemDTO() { Name = ingredient1.Name, Quantity = 3, Unit = "g" };

            List<ItemDTO> newItems = new() { item1 };

            var result = await controller.Update(newItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> returns <see cref="UnauthorizedObjectResult"/> when the user ID is missing.
        /// </summary>
        [Fact]
        public async Task Delete_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Delete([]);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> returns an empty list when a user with no ingredients attempts deletion.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserHasNoIngredients_ReturnsList()
        {
            User user = new User
            {
                Id = "Id 12",
                UserIngredients = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            List<string> deleteItems = new() { "Ingredient" };

            var result = await controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> returns the full ingredient list when an empty delete list is provided.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserHasDeleteListEmpty_ReturnsList()
        {
            User user = new User
            {
                Id = "Id 13",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 24, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 25, Name = "Bar" };

            UserIngredient ui1 = new UserIngredient { IngredientId = ingredient1.Id, Quantity = 1, Unit = "kg", UserId = user.Id };
            UserIngredient ui2 = new UserIngredient { IngredientId = ingredient2.Id, Quantity = 1, Unit = "kg", UserId = user.Id };

            user.UserIngredients.Add(ui1);
            user.UserIngredients.Add(ui2);

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete([]);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == ui1.Quantity && i.Unit == ui1.Unit);
            Assert.Contains(items, i => i.Name == ingredient2.Name && i.Quantity == ui2.Quantity && i.Unit == ui2.Unit);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> removes a specified ingredient from the user's inventory.
        /// Ensures that only the requested ingredient is deleted while others remain unchanged.
        /// </summary>
        [Fact]
        public async Task Delete_WhenIdIsValid_ReturnsList()
        {
            User user = new User
            {
                Id = "Id 14",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 26, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 27, Name = "Bar" };

            UserIngredient ui1 = new UserIngredient { IngredientId = ingredient1.Id, Quantity = 1, Unit = "kg", UserId = user.Id };
            UserIngredient ui2 = new UserIngredient { IngredientId = ingredient2.Id, Quantity = 1, Unit = "kg", UserId = user.Id };

            user.UserIngredients.Add(ui1);
            user.UserIngredients.Add(ui2);

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            List<string> deleteItems = new List<string>() { ingredient2.Name };

            var result = await controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Contains(items, i => i.Name == ingredient1.Name && i.Quantity == ui1.Quantity && i.Unit == ui1.Unit);
            Assert.DoesNotContain(items, i => i.Name == ingredient2.Name && i.Quantity == ui2.Quantity && i.Unit == ui2.Unit);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> removes all user ingredients when all are specified for deletion.
        /// Ensures that an empty list is returned after deletion.
        /// </summary>
        [Fact]
        public async Task Delete_WhenIdIsValidAndDeletesAllItems_ReturnsEmptyList()
        {
            User user = new User
            {
                Id = "Id 15",
                UserIngredients = new List<UserIngredient>()
            };

            Ingredient ingredient1 = new Ingredient { Id = 28, Name = "Foo" };
            Ingredient ingredient2 = new Ingredient { Id = 29, Name = "Bar" };

            UserIngredient ui1 = new UserIngredient { IngredientId = ingredient1.Id, Quantity = 1, Unit = "kg", UserId = user.Id };
            UserIngredient ui2 = new UserIngredient { IngredientId = ingredient2.Id, Quantity = 1, Unit = "kg", UserId = user.Id };

            user.UserIngredients.Add(ui1);
            user.UserIngredients.Add(ui2);

            _context.Ingredients.AddRange(ingredient1, ingredient2);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            List<string> deleteItems = new List<string>() { ingredient1.Name, ingredient2.Name };

            var result = await controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }
    }
}
