/// <summary>
/// This file contains the <c>InventoryControllerTests</c> class, which provides unit tests 
/// for the <see cref="InventoryController"/> class. These tests validate the functionality 
/// of retrieving, updating, and deleting user inventory items while handling authentication and data constraints.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-04</date>

using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;

namespace backendtest
{
    /// <summary>
    /// The <c>InventoryControllerTests</c> class contains unit tests for the <see cref="InventoryController"/> API.
    /// It verifies that user inventory retrieval, updates, and deletions work correctly, including authentication handling.
    /// </summary>
    public class InventoryControllerTests : IClassFixture<ICareContextFixture>, IAsyncLifetime
    {
        private readonly ICareServerContext _context;
        private InventoryController _controller;

        /// <summary>
        /// Initializes a new instance of the <c>InventoryControllerTests</c> class.
        /// Sets up the database context and controller for testing.
        /// </summary>
        /// <param name="fixture">The test fixture that provides an in-memory database context.</param>
        public InventoryControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<InventoryController>.Instance;
            _controller = new(_context, logger);
        }

        /// <summary>
        /// Clears existing users, ingredients, and user inventory items before running tests.
        /// Then, it seeds the database with sample ingredient data.
        /// </summary>
        public async Task InitializeAsync()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Ingredients.RemoveRange(_context.Ingredients);
            _context.UserIngredients.RemoveRange(_context.UserIngredients);
            await _context.SaveChangesAsync();

            _context.Ingredients.AddRange(
                new Ingredient { Id = 1, Name = "Arroz", Kcal = 124, Category = "Cereais e Derivados" },
                new Ingredient { Id = 2, Name = "Batata", Kcal = 137, Category = "Cereais e Derivados" },
                new Ingredient { Id = 3, Name = "Carne", Kcal = 200, Category = "Carne" }
            );

            var user1 = new User { Id = "User1", UserIngredients = new List<UserIngredient>() };
            var user2 = new User { Id = "User2", UserIngredients = new List<UserIngredient>() };

            user1.UserIngredients.Add(new UserIngredient { IngredientId = 1, Quantity = 200, Unit = "g", UserId = user1.Id });
            user1.UserIngredients.Add(new UserIngredient { IngredientId = 2, Quantity = 150, Unit = "g", UserId = user1.Id });

            user2.UserIngredients.Add(new UserIngredient { IngredientId = 3, Quantity = 100, Unit = "g", UserId = user2.Id });

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Handles asynchronous cleanup after tests (no action needed).
        /// </summary>
        public Task DisposeAsync() => Task.CompletedTask;

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
        /// Tests that <c>Get()</c> returns the correct list of user ingredients when a valid user ID is provided.
        /// </summary>
        [Fact]
        public async Task Get_WhenIdIsValid_ReturnsUserIngredients()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var ingredients = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(ingredients);
            Assert.Equal(2, ingredients.Count);
            Assert.Contains(ingredients, i => i.Name == "Arroz"  && i.Quantity == 200);
            Assert.Contains(ingredients, i => i.Name == "Batata"  && i.Quantity == 150);
        }

        /// <summary>
        /// Tests that when a user has no ingredients in their inventory, 
        /// the API correctly returns an empty list.
        /// </summary>
        [Fact]
        public async Task Get_WhenUserHasNoItems_ReturnsEmptyList()
        {
            var user = new User { Id = "EmptyUser", UserIngredients = new List<UserIngredient>() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        /// <summary>
        /// Tests that when a request to update the inventory is made 
        /// without a valid user ID (unauthenticated request), 
        /// the API returns an unauthorized response.
        /// </summary>
        [Fact]
        public async Task Update_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Update([]);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Update()</c> correctly updates a user's ingredient quantity and unit.
        /// </summary>
        [Fact]
        public async Task Update_WhenUserUpdatesIngredient_UpdatesSuccessfully()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            ItemDTO item1 = new ItemDTO { Name = "Arroz", Quantity = 300, Unit = "g" };
            ItemDTO item2 = new ItemDTO { Name = "Batata", Quantity = 100, Unit = "kg" };

            var updatedIngredients = new List<ItemDTO> { item1, item2 };

            var result = await _controller.Update(updatedIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Equal(2, updatedList.Count);
            Assert.Contains(updatedList, i => i.Name == item1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(updatedList, i => i.Name == item2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        /// <summary>
        /// Tests that when a user adds a new ingredient to their inventory, 
        /// it is successfully added alongside their existing ingredients.
        /// </summary>
        [Fact]
        public async Task Update_WhenAddingNewIngredient_AddsSuccessfully()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            ItemDTO item = new ItemDTO { Name = "Carne", Quantity = 500, Unit = "g" };

            var newIngredients = new List<ItemDTO> { item };

            var result = await _controller.Update(newIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Equal(3, updatedList.Count);
            Assert.Contains(updatedList, i => i.Name == "Arroz" && i.Quantity == 200 && i.Unit == "g");
            Assert.Contains(updatedList, i => i.Name == "Batata" && i.Quantity == 150 && i.Unit == "g");
            Assert.Contains(updatedList, i => i.Name == item.Name && i.Quantity == item.Quantity && i.Unit == item.Unit);
        }

        /// <summary>
        /// Tests that when a user attempts to add ingredients that do not exist in the database,
        /// those ingredients are not added, and the inventory remains unchanged.
        /// </summary>
        [Fact]
        public async Task Update_WhenUserAddsIngredientThatDontExist_UpdatesSuccessfully()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            ItemDTO item1 = new ItemDTO { Name = "Não Existe", Quantity = 100, Unit = "g" };
            ItemDTO item2 = new ItemDTO { Name = "Também Não Existe", Quantity = 100, Unit = "g" };

            var nonExistentIngredients = new List<ItemDTO> { item1, item2 };

            var result = await _controller.Update(nonExistentIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Equal(2, updatedList.Count);
            Assert.DoesNotContain(updatedList, i => i.Name == item1.Name && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.DoesNotContain(updatedList, i => i.Name == item2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.Contains(updatedList, i => i.Name == "Arroz" && i.Quantity == 200 && i.Unit == "g");
            Assert.Contains(updatedList, i => i.Name == "Batata" && i.Quantity == 150 && i.Unit == "g");
        }

        /// <summary>
        /// Tests that when a user provides an empty ingredient list for an update, 
        /// the system does not modify their existing ingredients.
        /// </summary>
        [Fact]
        public async Task Update_WhenUserUpdatesIngredientWithEmptyList_UpdatesSuccessfully()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var updatedIngredients = new List<ItemDTO>();

            var result = await _controller.Update(updatedIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Equal(2, updatedList.Count);
            Assert.Contains(updatedList, i => i.Name == "Arroz" && i.Quantity == 200 && i.Unit == "g");
            Assert.Contains(updatedList, i => i.Name == "Batata" && i.Quantity == 150 && i.Unit == "g");
        }

        /// <summary>
        /// Tests that when a user adds new ingredients, including duplicates, the system correctly 
        /// updates the inventory without storing repeated items.
        /// </summary>
        [Fact]
        public async Task Update_WhenAddingNewIngredientThatHaveRepeatedValues_AddsSuccessfullyWithoutRepeatedItems()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            ItemDTO item1 = new ItemDTO { Name = "Batata", Quantity = 300, Unit = "kg" };
            ItemDTO item2 = new ItemDTO { Name = "Carne", Quantity = 500, Unit = "g" };
            ItemDTO item3 = new ItemDTO { Name = "Batata", Quantity = 500, Unit = "g" };

            var newIngredients = new List<ItemDTO> { item1, item2, item3 };

            var result = await _controller.Update(newIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Equal(3, updatedList.Count);
            Assert.Contains(updatedList, i => i.Name == "Arroz" && i.Quantity == 200 && i.Unit == "g");
            Assert.Contains(updatedList, i => i.Name == item2.Name && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.Contains(updatedList, i => i.Name == item3.Name && i.Quantity == item3.Quantity && i.Unit == item3.Unit);
        }

        /// <summary>
        /// Tests that when a user with a null list of ingredients adds a new ingredient, 
        /// the system correctly initializes the ingredient list and adds the new ingredient.
        /// </summary>
        [Fact]
        public async Task Update_WhenAddingNewIngredientAndUserAsNullList_AddsSuccessfully()
        {
            var user = new User { Id = "EmptyUser", UserIngredients = null };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            ItemDTO item = new ItemDTO { Name = "Carne", Quantity = 500, Unit = "g" };

            var newIngredients = new List<ItemDTO> { item };

            var result = await _controller.Update(newIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Single(updatedList);
            Assert.Contains(updatedList, i => i.Name == item.Name && i.Quantity == item.Quantity && i.Unit == item.Unit);
        }

        /// <summary>
        /// Tests that when a user with an empty ingredient list adds a new ingredient, 
        /// it is successfully added to their inventory.
        /// </summary>
        [Fact]
        public async Task Update_WhenAddingNewIngredientAndUserAsEmptyList_AddsSuccessfully()
        {
            var user = new User { Id = "EmptyUser", UserIngredients = new List<UserIngredient>() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            ItemDTO item = new ItemDTO { Name = "Carne", Quantity = 500, Unit = "g" };

            var newIngredients = new List<ItemDTO> { item };

            var result = await _controller.Update(newIngredients);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var updatedList = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(updatedList);
            Assert.Single(updatedList);
            Assert.Contains(updatedList, i => i.Name == item.Name && i.Quantity == item.Quantity && i.Unit == item.Unit);
        }

        /// <summary>
        /// Tests that when a user who is not authenticated (UserId is null) attempts to delete an ingredient, 
        /// the API returns an unauthorized response.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Delete([]);

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Delete()</c> removes an ingredient from the user's inventory successfully.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserDeletesIngredient_RemovesItSuccessfully()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var deleteItems = new List<string> { "Batata" };

            var result = await _controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var remainingItems = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(remainingItems);
            Assert.Single(remainingItems);
            Assert.DoesNotContain(remainingItems, i => i.Name == "Batata");
        }

        /// <summary>
        /// Tests that <c>Delete()</c> removes all ingredients from the user's inventory when all are selected.
        /// </summary>
        [Fact]
        public async Task Delete_WhenDeletingAllIngredients_ReturnsEmptyList()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var deleteItems = new List<string> { "Arroz", "Batata" };

            var result = await _controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var remainingItems = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(remainingItems);
            Assert.Empty(remainingItems);
        }

        /// <summary>
        /// Tests that calling <c>Delete()</c> with an empty list does not remove any items from the user's inventory.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserDoesntDeleteAnIngredient_DoesntRemoveAnything()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var deleteItems = new List<string>();

            var result = await _controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var remainingItems = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(remainingItems);
            Assert.Equal(2, remainingItems.Count);
            Assert.Contains(remainingItems, i => i.Name == "Arroz");
            Assert.Contains(remainingItems, i => i.Name == "Batata");
        }

        /// <summary>
        /// Tests that calling <c>Delete()</c> on a user with no ingredients results in an empty response.
        /// Ensures the API correctly handles users with an empty inventory.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserHasNoIngredients_ReturnsEmptyList()
        {
            var user = new User { Id = "EmptyUser", UserIngredients = new List<UserIngredient>() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            var deleteItems = new List<string>();

            var result = await _controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var remainingItems = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(remainingItems);
            Assert.Empty(remainingItems);
        }

        /// <summary>
        /// Tests that calling <c>Delete()</c> with an ingredient the user does not own does not remove any valid ingredients.
        /// The user’s existing inventory should remain unchanged.
        /// </summary>
        [Fact]
        public async Task Delete_WhenUserDeletesIngredientHeDoesntHave_ReturnsNormalList()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var deleteItems = new List<string> { "Carne" };

            var result = await _controller.Delete(deleteItems);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<ItemDTO>>>(result);

            var remainingItems = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<ItemDTO>;
            Assert.NotNull(remainingItems);
            Assert.Equal(2, remainingItems.Count);
            Assert.Contains(remainingItems, i => i.Name == "Arroz");
            Assert.Contains(remainingItems, i => i.Name == "Batata");
        }
    }
}
