/// <summary>
/// This file defines the <c>PublicUserController</c> class, responsible for managing the public profiles of authenticated users.
/// It provides API endpoints for retrieving and updating user profile data based on authentication tokens.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-05</date>

using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    /// <summary>
    /// Controller <c>UserController</c> manages the public profiles of authenticated users.
    /// It allows users to retrieve and update their public profile information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>UserController</c> class.
        /// </summary>
        /// <param name="context">The database context for accessing user data.</param>
        /// <param name="logger">The logger instance for logging application activity.</param>
        public UserController(ICareServerContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the public profile of the authenticated user.
        /// </summary>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>PublicUser</c> object if found, or an error response otherwise.
        /// </returns>
        [HttpGet("")]
        public async Task<ActionResult<UserDTO>> Get()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();

                var categories = await _context.Recipes.Where(r => r.Category != null)
                                                       .Select(r => r.Category!)
                                                       .Distinct()
                                                       .AsNoTracking()
                                                       .ToListAsync();

                return Ok(new UserDTO(user, null, categories));
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves the permissions associated with the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a <see cref="PermissionsDTO"/> with the user's permissions,
        /// or an error response if the request fails.
        /// </returns>
        [HttpGet("permissions")]
        public async Task<ActionResult<PermissionsDTO>> GetPermissions()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();

                var permissions = new PermissionsDTO
                {
                    Notifications = user.Notifications,
                    Preferences = user.Preferences?.Any() ?? false,
                    Restrictions = user.Restrictions?.Any() ?? false,
                    Inventory = user.UserIngredients?.Any() ?? false,
                };

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<PermissionsDTO>> GetPreferences()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();

                var preferences = user.Preferences;

                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user's preferences");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("restrictions")]
        public async Task<ActionResult<PermissionsDTO>> GetRestrictions()
        {
            try
            {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();

                var restrictions = user.Restrictions;

                return Ok(restrictions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user's restrictions");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates the public profile of the authenticated user.
        /// </summary>
        /// <param name="model">The <c>PublicUser</c> model containing the updated profile data.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the updated <c>PublicUser</c> object if successful, or an error response otherwise.
        /// </returns>
        [HttpPut("")]
        public async Task<ActionResult<UserDTO>> Edit([FromBody] UserDTO model)
        {
            if (model == null) return BadRequest("Invalid data provided.");

            try {
                var id = User.FindFirst("UserId")?.Value;
                if (id == null) return Unauthorized("User ID not found in token.");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return NotFound();

                if (model.Email != user.Email)
                {
                    bool isUnique = await IsEmailUnique(model.Email);
                    if (!isUnique) model.Email = user.Email;
                }

                var categories = await _context.Recipes.Where(r => r.Category != null)
                                                       .Select(r => r.Category!)
                                                       .Distinct()
                                                       .AsNoTracking()
                                                       .ToListAsync();

                model.Preferences.FixCollection(categories);
                model.Restrictions.FixCollection(categories);

                user.UpdateFromModel(model);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} updated their information.", user.Id);

                return Ok(new UserDTO(user, model, categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Checks whether the provided email is unique in the system.
        /// </summary>
        /// <param name="email">The email to check for uniqueness.</param>
        /// <returns>
        /// <c>true</c> if the email is unique; otherwise, <c>false</c>.
        /// </returns>
        private async Task<bool> IsEmailUnique(string? email)
        {
            if (email == null) return false;

            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}


