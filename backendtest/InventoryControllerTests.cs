using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Ingredients;

/// <summary>
/// This file contains unit tests for the <c>InventoryController</c> class.
/// These tests ensure that the controller functions correctly in various scenarios, 
/// including inventory modifications or retrieval.
/// The tests verify expected behavior under different conditions.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais - 202001541</author>
/// <date>Last Modified: 2025-02-19</date>
namespace backendtest
{
    /// <summary>
    /// Test class for the <c>InventoryController</c>.
    /// This class contains unit tests to validate the functionality of inventory management, 
    /// ensuring correct handling of data retrieval, updates, and business logic enforcement.
    /// </summary>
    public class InventoryControllerTests : IClassFixture<ICareContextFixture>
    {
        private readonly ICareServerContext _context;
        private InventoryController controller;

        public InventoryControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<InventoryController>.Instance;

            controller = new(_context, logger);
        }

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

        [Fact]
        public async Task Get_WhenIdIsInvalid_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("InvalidId", controller);

            var result = await controller.Get();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Get_WhenIdIsValid_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 4",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(user.UserItems.Count, items.Count);
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);           
        }

        [Fact]
        public async Task Get_WhenUserHasNoItems_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 5",
                UserItems = new List<UserIngredient>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public async Task Update_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Update(new List<PublicItem>());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_WhenIdIsInvalid_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("InvalidId", controller);

            var result = await controller.Update(new List<PublicItem>());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasNullListOfItems_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 6",
                UserItems = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            var result = await controller.Update(new List<PublicItem> { new PublicItem { Name = item1.ItemName, Quantity = item1.Quantity, Unit = item1.Unit },
                                                                        new PublicItem { Name = item2.ItemName, Quantity = item2.Quantity, Unit = item2.Unit },});

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasEmptyListOfItems_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 7",
                UserItems = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            var result = await controller.Update(new List<PublicItem> { new PublicItem { Name = item1.ItemName, Quantity = item1.Quantity, Unit = item1.Unit },
                                                                        new PublicItem { Name = item2.ItemName, Quantity = item2.Quantity, Unit = item2.Unit },});

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasListOfItems_ReturnsEmptyListOfItems()
        {

            User user = new User
            {
                Id = "Id 8",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            UserIngredient item3 = new UserIngredient { ItemName = "Meat", Quantity = 1, Unit = "l", User = user, UserId = user.Id };

            var result = await controller.Update(new List<PublicItem> { new PublicItem { Name = item3.ItemName, Quantity = item3.Quantity, Unit = item3.Unit } });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(3, items.Count);
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.Contains(items, i => i.Name == item3.ItemName && i.Quantity == item3.Quantity && i.Unit == item3.Unit);
        }

        [Fact]
        public async Task Update_WhenIdIsValidAndUserHasListOfItemsAndItIsSentRepeatedItems_ReturnsEmptyListOfItems()
        {

            User user = new User
            {
                Id = "Id 9",
                UserItems = new List<UserIngredient>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            UserIngredient item1 = new UserIngredient { ItemName = "Meat", Quantity = 1, Unit = "l", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "ml", User = user, UserId = user.Id };
            UserIngredient item3 = new UserIngredient { ItemName = "Meat", Quantity = 4, Unit = "ml", User = user, UserId = user.Id };

            var result = await controller.Update(new List<PublicItem> { new PublicItem { Name = item1.ItemName, Quantity = item1.Quantity, Unit = item1.Unit },
                                                                        new PublicItem { Name = item2.ItemName, Quantity = item2.Quantity, Unit = item2.Unit },
                                                                        new PublicItem { Name = item3.ItemName, Quantity = item3.Quantity, Unit = item3.Unit } });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.Contains(items, i => i.Name == item3.ItemName && i.Quantity == item3.Quantity && i.Unit == item3.Unit);
        }

        [Fact]
        public async Task Update_WhenIdIsValidAndUserUpdatesAnItem_ReturnsEmptyListOfItems()
        {

            User user = new User
            {
                Id = "Id 10",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            UserIngredient item3 = new UserIngredient { ItemName = "Potato", Quantity = 10, Unit = "g", User = user, UserId = user.Id };

            var result = await controller.Update(new List<PublicItem> { new PublicItem { Name = item3.ItemName, Quantity = item3.Quantity, Unit = item3.Unit } });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.Contains(items, i => i.Name == item3.ItemName && i.Quantity == item3.Quantity && i.Unit == item3.Unit);
        }

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

        [Fact]
        public async Task Delete_WhenIdIsInvalid_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("InvalidId", controller);

            var result = await controller.Delete([]);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_WhenUserHasNoItems_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 11",
                UserItems = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete([]);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public async Task Delete_WhenUserHasAnListIsEmpty_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 12",
                UserItems = new List<UserIngredient>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete([]);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }

        [Fact]
        public async Task Delete_WhenIdIsValid_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 13",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<string> { item1.ItemName });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
            Assert.DoesNotContain(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
        }

        [Fact]
        public async Task Delete_WhenIdIsValidAndListWithItemToDeleteIsEmpty_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 14",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete([]);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity && i.Unit == item1.Unit);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity && i.Unit == item2.Unit);
        }

        [Fact]
        public async Task Delete_WhenIdIsValidAndDeletesAllItems_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 15",
                UserItems = new List<UserIngredient>()
            };

            UserIngredient item1 = new UserIngredient { ItemName = "Potato", Quantity = 1, Unit = "kg", User = user, UserId = user.Id };
            UserIngredient item2 = new UserIngredient { ItemName = "Tomato", Quantity = 5, Unit = "ml", User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<string> {item1.ItemName, item2.ItemName });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }
    }
}
