using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using backend.Models.Data_Transfer_Objects;

namespace backendtest
{
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
                UserItems = new List<UserItem>()
            };

            UserItem item1 = new UserItem { ItemName = "Potato", Quantity = 1, User = user, UserId = user.Id };
            UserItem item2 = new UserItem { ItemName = "Tomato", Quantity = 5, User = user, UserId = user.Id };

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
            Assert.Contains(items, i => i.Name == item1.ItemName && i.Quantity == item1.Quantity);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity);           
        }

        [Fact]
        public async Task Get_WhenUserHasNoItems_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 5",
                UserItems = new List<UserItem>()
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
        public async Task Update_WhenIdIsValid_ReturnsListOfItems()
        {
            
        }

        [Fact]
        public async Task Update_WhenUserHasNoItems_ReturnsEmptyListOfItems()
        {
            
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Delete(new List<PublicItem>());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Delete_WhenIdIsInvalid_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("InvalidId", controller);

            var result = await controller.Delete(new List<PublicItem>());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_WhenUserHasNoItems_ReturnsEmptyListOfItems()
        {
            User user = new User
            {
                Id = "Id 8",
                UserItems = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<PublicItem>());

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
                Id = "Id 9",
                UserItems = new List<UserItem>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<PublicItem>());

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
                Id = "Id 10",
                UserItems = new List<UserItem>()
            };

            UserItem item1 = new UserItem { ItemName = "Potato", Quantity = 1, User = user, UserId = user.Id };
            UserItem item2 = new UserItem { ItemName = "Tomato", Quantity = 5, User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<PublicItem> { new PublicItem { Name = item1.ItemName, Quantity = item1.Quantity } });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Single(items);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity);
            Assert.DoesNotContain(items, i => i.Name == item1.ItemName && i.Quantity == item2.Quantity);
        }

        [Fact]
        public async Task Delete_WhenIdIsValidAndListWithItemToDeleteIsEmpty_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 11",
                UserItems = new List<UserItem>()
            };

            UserItem item1 = new UserItem { ItemName = "Potato", Quantity = 1, User = user, UserId = user.Id };
            UserItem item2 = new UserItem { ItemName = "Tomato", Quantity = 5, User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<PublicItem>());

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Equal(2, items.Count);
            Assert.Contains(items, i => i.Name == item2.ItemName && i.Quantity == item2.Quantity);
            Assert.DoesNotContain(items, i => i.Name == item1.ItemName && i.Quantity == item2.Quantity);
        }

        [Fact]
        public async Task Delete_WhenIdIsValidAndDeletesAllItems_ReturnsListOfItems()
        {
            User user = new User
            {
                Id = "Id 11",
                UserItems = new List<UserItem>()
            };

            UserItem item1 = new UserItem { ItemName = "Potato", Quantity = 1, User = user, UserId = user.Id };
            UserItem item2 = new UserItem { ItemName = "Tomato", Quantity = 5, User = user, UserId = user.Id };

            user.UserItems.Add(item1);
            user.UserItems.Add(item2);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, controller);

            var result = await controller.Delete(new List<PublicItem> { new PublicItem { Name = item1.ItemName, Quantity = item1.Quantity },
                                                                        new PublicItem { Name = item2.ItemName, Quantity = item2.Quantity } });

            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<PublicItem>>>(result);

            var items = Assert.IsType<OkObjectResult>(result.Result)?.Value as List<PublicItem>;
            Assert.NotNull(items);
            Assert.Empty(items);
        }
    }
}
