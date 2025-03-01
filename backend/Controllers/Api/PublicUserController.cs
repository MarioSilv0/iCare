using backend.Data;
using backend.Models;
using backend.Models.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// This file defines the <c>PublicUserController</c> class, responsible for managing public user profiles.
/// It provides API endpoints for retrieving and updating user data based on authentication tokens.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-02-06</date>
namespace backend.Controllers.Api
{
    /// <summary>
    /// Controller <c>PublicUserController</c> manages public user profiles.
    /// It allows authenticated users to retrieve and update their profile information.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PublicUserController : ControllerBase
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<PublicUserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>PublicUserController</c> class.
        /// </summary>
        /// <param name="context">The database context for accessing user data.</param>
        /// <param name="logger">The logger instance for logging application activity.</param>
        public PublicUserController(ICareServerContext context, ILogger<PublicUserController> logger)
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
        public async Task<ActionResult<PublicUser>> Get()
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

                return Ok(new PublicUser(user, null, categories));
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving user");
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
        public async Task<ActionResult<PublicUser>> Edit([FromBody] PublicUser model)
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

                return Ok(new PublicUser(user, model, categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private async Task<bool> IsEmailUnique(string? email)
        {
            if (email == null) return false;

            return !await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}


