using backend.Controllers;
using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;

/// <summary>
/// This file contains unit tests for the <c>PublicUserController</c> class, specifically testing the <c>Edit</c> method.
/// These tests ensure that the controller behaves correctly under various conditions such as valid/invalid tokens, empty preferences, and restrictions.
/// The tests validate the return types and the correctness of the resulting <c>PublicUser</c> object.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais - 202001541</author>
/// <date>Last Modified: 2025-01-30</date>

namespace backendtest
{
    /// <summary>
    /// Test class for the <c>PublicUserController</c>.
    /// This class contains tests for the <c>Edit</c> method, checking various scenarios involving user preferences, restrictions, and token validation.
    /// </summary>
    public class PublicUserControllerTests : IClassFixture<ICareContextFixture>
    {
        private readonly ICareServerContext _context;
        private PublicUserController controller;

        public PublicUserControllerTests(ICareContextFixture fixture) 
        {
            _context = fixture.DbContext;
            var logger = NullLogger<PublicUserController>.Instance;

            controller = new(_context, logger);
        }

        [Fact]
        public async Task Edit_WhenIdIsNull_ReturnsUnauthorized()
        {
            // Do not set user ID in claims (simulate missing token)
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await controller.Edit();

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Edit_WhenIdIsInvalid_ReturnsNotFound()
        {
            SetUserIdClaim("InvalidId");

            var result = await controller.Edit();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Edit_WhenIdIsValid_ReturnsPublicUser()
        {
            User user = new User
            {
                Id = "Id",
                Email = "user@email.com",
                Name = "User",
                UserPreferences = new List<UserPreference>(),
                UserRestrictions = new List<UserRestriction>()
            };

            user.UserPreferences.Add(new UserPreference { UserId = user.Id, PreferenceId = 1 });
            user.UserPreferences.Add(new UserPreference { UserId = user.Id, PreferenceId = 2 });
            user.UserRestrictions.Add(new UserRestriction { UserId = user.Id, RestrictionId = 2 });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetUserIdClaim(user.Id);

            var result = await controller.Edit();

            Assert.NotNull(result);
            Assert.IsType<ActionResult<PublicUser>>(result);

            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(publicUser);
            Assert.Equal(user.Email, publicUser.Email);
            Assert.Equal(user.Name, publicUser.Name);
            Assert.NotNull(publicUser.Preferences);
            Assert.Equal(4, publicUser.Preferences.Count);
            Assert.Contains(publicUser.Preferences, p => p.Id == 1 && p.IsSelected);
            Assert.Contains(publicUser.Preferences, p => p.Id == 2 && p.IsSelected);
            Assert.Contains(publicUser.Preferences, p => p.Id == 3 && !p.IsSelected);
            Assert.Contains(publicUser.Preferences, p => p.Id == 4 && !p.IsSelected);
            Assert.NotNull(publicUser.Restrictions);
            Assert.Equal(2, publicUser.Restrictions.Count);
            Assert.Contains(publicUser.Restrictions, r => r.Id == 1 && !r.IsSelected);
            Assert.Contains(publicUser.Restrictions, r => r.Id == 2 && r.IsSelected);
        }

        [Fact]
        public async Task Edit_WhenUserHasNoPreferencesOrRestrictions_ReturnsPublicUser()
        {
            User user = new User
            {
                Id = "Id 2",
                Email = "user@email.com",
                Name = "User",
                UserPreferences = [],
                UserRestrictions = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetUserIdClaim(user.Id);

            var result = await controller.Edit();

            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(publicUser);
            Assert.Equal(4, publicUser.Preferences.Count);
            Assert.All(publicUser.Preferences, pu => Assert.False(pu.IsSelected));
            Assert.Equal(2, publicUser.Restrictions.Count);
            Assert.All(publicUser.Restrictions, pu => Assert.False(pu.IsSelected));
        }

        [Fact]
        public async Task Edit_WhenPreferencesAndRestrictionsAreEmpty_ReturnsPublicUser()
        {
            User user = new User
            {
                Id = "Id 3",
                Email = "user@email.com",
                Name = "User",
                UserPreferences = [],
                UserRestrictions = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Ensure preferences and restrictions lists are empty
            var p = _context.Preferences.ToList();
            var r = _context.Restrictions.ToList();

            _context.Preferences.RemoveRange(_context.Preferences);
            _context.Restrictions.RemoveRange(_context.Restrictions);
            await _context.SaveChangesAsync();

            SetUserIdClaim(user.Id);

            var result = await controller.Edit();

            var publicUser = Assert.IsType<OkObjectResult>(result.Result)?.Value as PublicUser;
            Assert.NotNull(publicUser);
            Assert.Empty(publicUser.Preferences);
            Assert.Empty(publicUser.Restrictions);

            _context.Preferences.AddRange(p);
            _context.Restrictions.AddRange(r);
        }

        private void SetUserIdClaim(string userId)
        {
            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }
    }
}
