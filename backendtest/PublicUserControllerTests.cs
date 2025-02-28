//using backend.Controllers;
//using backend.Controllers.Api;
//using backend.Data;
//using backend.Models;
//using backend.Models.Preferences;
//using backend.Models.Restrictions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging.Abstractions;
//using System.Security.Claims;

///// <summary>
///// This file contains unit tests for the <c>PublicUserController</c> class, specifically testing the <c>Edit</c> method.
///// These tests ensure that the controller behaves correctly under various conditions such as valid/invalid tokens, empty preferences, and restrictions.
///// The tests validate the return types and the correctness of the resulting <c>PublicUser</c> object.
///// </summary>
///// <author>Luís Martins - 202100239</author>
///// <author>João Morais - 202001541</author>
///// <date>Last Modified: 2025-01-30</date>

//namespace backendtest
//{
//    /// <summary>
//    /// Test class for the <c>PublicUserController</c>.
//    /// This class contains tests for the <c>Edit</c> method, checking various scenarios involving user preferences, restrictions, and token validation.
//    /// </summary>
//    public class PublicUserControllerTests : IClassFixture<ICareContextFixture>
//    {
//        private readonly ICareServerContext _context;
//        private PublicUserController controller;

//        public PublicUserControllerTests(ICareContextFixture fixture) 
//        {
//            _context = fixture.DbContext;
//            var logger = NullLogger<PublicUserController>.Instance;

//            controller = new(_context, logger);
//        }

//        [Fact]
//        public async Task Get_WhenIdIsNull_ReturnsUnauthorized()
//        {
//            // Do not set user ID in claims (simulate missing token)
//            controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
//            };

//            var result = await controller.Get();

//            Assert.IsType<UnauthorizedObjectResult>(result.Result);
//        }

//        [Fact]
//        public async Task Get_WhenIdIsInvalid_ReturnsNotFound()
//        {
//            Authenticate.SetUserIdClaim("InvalidId", controller);

//            var result = await controller.Get();

//            Assert.IsType<NotFoundResult>(result.Result);
//        }

//        [Fact]
//        public async Task Get_WhenIdIsValid_ReturnsPublicUser()
//        {
//            User user = new User
//            {
//                Id = "Id",
//                Email = "user@email.com",
//                Name = "User",
//                UserPreferences = new List<UserPreference>(),
//                UserRestrictions = new List<UserRestriction>()
//            };

//            user.UserPreferences.Add(new UserPreference { UserId = user.Id, PreferenceId = 1 });
//            user.UserPreferences.Add(new UserPreference { UserId = user.Id, PreferenceId = 2 });
//            user.UserRestrictions.Add(new UserRestriction { UserId = user.Id, RestrictionId = 2 });

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            Authenticate.SetUserIdClaim(user.Id, controller);

//            var result = await controller.Get();

//            Assert.NotNull(result);
//            Assert.IsType<ActionResult<PublicUser>>(result);

//            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
//            Assert.NotNull(publicUser);
//            Assert.Equal(user.Email, publicUser.Email);
//            Assert.Equal(user.Name, publicUser.Name);
//            Assert.NotNull(publicUser.Preferences);
//            Assert.Equal(4, publicUser.Preferences.Count);
//            Assert.Contains(publicUser.Preferences, p => p.Id == 1 && p.IsSelected);
//            Assert.Contains(publicUser.Preferences, p => p.Id == 2 && p.IsSelected);
//            Assert.Contains(publicUser.Preferences, p => p.Id == 3 && !p.IsSelected);
//            Assert.Contains(publicUser.Preferences, p => p.Id == 4 && !p.IsSelected);
//            Assert.NotNull(publicUser.Restrictions);
//            Assert.Equal(2, publicUser.Restrictions.Count);
//            Assert.Contains(publicUser.Restrictions, r => r.Id == 1 && !r.IsSelected);
//            Assert.Contains(publicUser.Restrictions, r => r.Id == 2 && r.IsSelected);
//        }

//        [Fact]
//        public async Task Get_WhenUserHasNoPreferencesOrRestrictions_ReturnsPublicUser()
//        {
//            User user = new User
//            {
//                Id = "Id 2",
//                Email = "user@email.com",
//                Name = "User",
//                UserPreferences = [],
//                UserRestrictions = []
//            };
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            Authenticate.SetUserIdClaim(user.Id, controller);

//            var result = await controller.Get();

//            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
//            Assert.NotNull(publicUser);
//            Assert.Equal(4, publicUser.Preferences.Count);
//            Assert.All(publicUser.Preferences, pu => Assert.False(pu.IsSelected));
//            Assert.Equal(2, publicUser.Restrictions.Count);
//            Assert.All(publicUser.Restrictions, pu => Assert.False(pu.IsSelected));
//        }

//        [Fact]
//        public async Task Get_WhenPreferencesAndRestrictionsAreEmpty_ReturnsPublicUser()
//        {
//            User user = new User
//            {
//                Id = "Id 3",
//                Email = "user@email.com",
//                Name = "User",
//                UserPreferences = [],
//                UserRestrictions = []
//            };
//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            // Ensure preferences and restrictions lists are empty
//            var p = _context.Preferences.ToList();
//            var r = _context.Restrictions.ToList();

//            _context.Preferences.RemoveRange(_context.Preferences);
//            _context.Restrictions.RemoveRange(_context.Restrictions);
//            await _context.SaveChangesAsync();

//            Authenticate.SetUserIdClaim(user.Id, controller);

//            var result = await controller.Get();

//            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
//            Assert.NotNull(publicUser);
//            Assert.Empty(publicUser.Preferences);
//            Assert.Empty(publicUser.Restrictions);

//            _context.Preferences.AddRange(p);
//            _context.Restrictions.AddRange(r);
//        }

//        [Fact]
//        public async Task Edit_WhenModelIsNull_ReturnsBadRequest()
//        {
//            Authenticate.SetUserIdClaim("some-user-id", controller);
//            PublicUser? pu = null;
//            var result = await controller.Edit(pu);

//            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
//            Assert.Equal("Invalid data provided.", badRequest.Value);
//        }

//        [Fact]
//        public async Task Edit_WhenIdIsNull_ReturnsUnauthorized()
//        {
//            // Do not set user ID in claims (simulate missing token)
//            controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
//            };

//            var model = new PublicUser { Email = "test@email.com" };
//            var result = await controller.Edit(model);

//            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
//            Assert.Equal("User ID not found in token.", unauthorized.Value);
//        }

//        [Fact]
//        public async Task Edit_WhenIdIsInvalid_ReturnsNotFound()
//        {
//            Authenticate.SetUserIdClaim("InvalidId", controller);

//            var model = new PublicUser { Email = "test@email.com" };
//            var result = await controller.Edit(model);

//            Assert.IsType<NotFoundResult>(result.Result);
//        }

//        [Fact]
//        public async Task Edit_WhenEmailIsNotUnique_KeepsOldEmail()
//        {
//            User user1 = new User
//            {
//                Id = "User1",
//                Name = "User1",
//                Email = "user1@email.com",                
//            };

//            User user2 = new User
//            {
//                Id = "User2",
//                Name = "User2",
//                Email = "user2@email.com",
//            };

//            _context.Users.Add(user1);
//            _context.Users.Add(user2);
//            await _context.SaveChangesAsync();

//            Authenticate.SetUserIdClaim(user1.Id, controller);
//            var model = new PublicUser { Email = user2.Email };

//            var result = await controller.Edit(model);

//            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;

//            Assert.NotNull(publicUser);
//            Assert.Equal(user1.Email, publicUser.Email);
//        }

//        [Fact]
//        public async Task Edit_WhenValidUser_UpdatesSuccessfully()
//        {
//            User user = new User
//            {
//                Id = "User",
//                Name = "User",
//                Email = "user@email.com",
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            Authenticate.SetUserIdClaim(user.Id, controller);
//            var model = new PublicUser { Email = "new@email.com", Name = "New Name" };

//            var result = await controller.Edit(model);
//            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;

//            Assert.NotNull(publicUser);
//            Assert.Equal(model.Email, publicUser.Email);
//            Assert.Equal(model.Name, publicUser.Name);
//        }
//    }
//}
