/// <summary>
/// This file contains the <c>PublicUserControllerTests</c> class, which provides unit tests 
/// for the <see cref="PublicUserController"/> class. These tests validate the functionality 
/// of retrieving and updating public user profiles while handling authentication and data constraints.
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
using backend.Models.Recipes;

namespace backendtest
{
    /// <summary>
    /// The <c>PublicUserControllerTests</c> class contains unit tests for the <see cref="PublicUserController"/> API.
    /// It verifies that user profile retrieval and updates work correctly, including authentication handling.
    /// </summary>
    public class PublicUserControllerTests : IClassFixture<ICareContextFixture>, IAsyncLifetime
    {
        private readonly ICareServerContext _context;
        private PublicUserController _controller;

        /// <summary>
        /// Initializes a new instance of the <c>PublicUserControllerTests</c> class.
        /// Sets up the database context and controller for testing.
        /// </summary>
        /// <param name="fixture">The test fixture that provides an in-memory database context.</param>
        public PublicUserControllerTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            var logger = NullLogger<PublicUserController>.Instance;
            _controller = new(_context, logger);
        }

        /// <summary>
        /// Clears existing users and recipes before running tests.
        /// </summary>
        public async Task InitializeAsync()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.Recipes.RemoveRange(_context.Recipes);
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
        /// Tests that <c>Get()</c> returns <see cref="NotFoundResult"/> when the user does not exist.
        /// </summary>
        [Fact]
        public async Task Get_WhenUserDoesNotExist_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("NonExistingUser", _controller);

            var result = await _controller.Get();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Get()</c> returns the correct public user profile when the user exists.
        /// </summary>
        [Fact]
        public async Task Get_WhenUserExists_ReturnsPublicUser()
        {
            var user = new User { Id = "User1", Email = "user@example.com", Preferences = new List<string>(), Restrictions = new List<string>() };
            var recipe1 = new Recipe { Name = "Recipe1", Category = "Dessert" };
            var recipe2 = new Recipe { Name = "Recipe2", Category = "Vegan" };

            user.Preferences.Add(recipe1.Category);
            user.Restrictions.Add(recipe2.Category);

            _context.Users.Add(user);
            _context.Recipes.AddRange(recipe1, recipe2);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            var result = await _controller.Get();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<PublicUser>>(result);

            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(publicUser);
            Assert.Equal(user.Email, publicUser.Email);
            Assert.Single(publicUser.Preferences);
            Assert.Contains(publicUser.Preferences, p => p == recipe1.Category);
            Assert.Single(publicUser.Restrictions);
            Assert.Contains(publicUser.Restrictions, r => r == recipe2.Category);
            Assert.Equal(2, publicUser.Categories.Count);
            Assert.Contains(publicUser.Categories, p => p == recipe1.Category);
            Assert.Contains(publicUser.Categories, p => p == recipe2.Category);
        }

        /// <summary>
        /// Tests that <c>Edit()</c> returns <see cref="UnauthorizedObjectResult"/> when no user ID is provided.
        /// </summary>
        [Fact]
        public async Task Edit_WhenUserIdIsNull_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.Edit(new PublicUser());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Edit()</c> returns <see cref="NotFoundResult"/> when the user does not exist.
        /// </summary>
        [Fact]
        public async Task Edit_WhenUserDoesNotExist_ReturnsNotFound()
        {
            Authenticate.SetUserIdClaim("NonExistingUser", _controller);

            var result = await _controller.Edit(new PublicUser());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Edit()</c> returns <see cref="BadRequestObjectResult"/> when the input model is null.
        /// </summary>
        [Fact]
        public async Task Edit_WhenModelIsNull_ReturnsBadRequest()
        {
            Authenticate.SetUserIdClaim("User1", _controller);

            var result = await _controller.Edit(null);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Tests that <c>Edit()</c> correctly updates user profile information.
        /// </summary>
        [Fact]
        public async Task Edit_WhenUserUpdatesProfile_ReturnsUpdatedPublicUser()
        {
            var user = new User { Id = "User1", Email = "old@example.com" };
            var recipe1 = new Recipe { Name = "Recipe1", Category = "Gluten-Free" };
            var recipe2 = new Recipe { Name = "Recipe2", Category = "Vegan" };

            _context.Users.Add(user);
            _context.Recipes.AddRange(recipe1, recipe2);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user.Id, _controller);

            var updatedModel = new PublicUser
            {
                Picture = "picture",
                Name = "name",
                Email = "new@example.com",
                Height = 1.5f,
                Weight = 50,
                Notifications = true,
                Preferences = new List<string> { "Vegan" },
                Restrictions = new List<string> { "Gluten-Free" }
            };

            var result = await _controller.Edit(updatedModel);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<PublicUser>>(result);

            var updatedUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(updatedUser);
            Assert.Equal(updatedModel.Picture, updatedUser.Picture);
            Assert.Equal(updatedModel.Name, updatedUser.Name);
            Assert.Equal(updatedModel.Email, updatedUser.Email);
            Assert.Equal(updatedModel.Height, updatedUser.Height);
            Assert.Equal(updatedModel.Weight, updatedUser.Weight);
            Assert.Single(updatedUser.Preferences);
            Assert.Contains(updatedUser.Preferences, p => p == updatedModel.Preferences.First());
            Assert.Single(updatedUser.Restrictions);
            Assert.Contains(updatedUser.Restrictions, p => p == updatedModel.Restrictions.First());
        }

        /// <summary>
        /// Tests that <c>Edit()</c> prevents updating email if it already exists for another user.
        /// </summary>
        [Fact]
        public async Task Edit_WhenEmailAlreadyExists_DoesNotChangeEmail()
        {
            var user1 = new User { Id = "User1", Email = "user1@example.com" };
            var user2 = new User { Id = "User2", Email = "user2@example.com" };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            Authenticate.SetUserIdClaim(user1.Id, _controller);

            var updatedModel = new PublicUser { Email = user2.Email };

            var result = await _controller.Edit(updatedModel);

            Assert.NotNull(result);
            Assert.IsType<ActionResult<PublicUser>>(result);

            var updatedUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(updatedUser);
            Assert.Equal("user1@example.com", updatedUser.Email);
        }
    }
}
