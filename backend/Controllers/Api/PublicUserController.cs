using backend.Data;
using backend.Models;
using backend.Models.Extensions;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// This file defines the <c>PublicUserController</c> class, which provides
/// API endpoints for managing public user profiles, including retrieving and
/// updating user data.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

namespace backend.Controllers.Api
{
    /// <summary>
    /// Controller <c>PublicUserController</c> provides endpoints for interacting with
    /// public user profiles, including retrieving and updating user data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublicUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PublicUserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <c>PublicUserController</c> class.
        /// </summary>
        /// <param name="context">The database context for interacting with the database.</param>
        /// <param name="logger">The logger for recording application activity.</param>
        public PublicUserController(ApplicationDbContext context, ILogger<PublicUserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the public profile of a user based on their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the <c>PublicUser</c> object or an error response.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PublicUser>> Edit(string id)
        {
            try
            {
                var user = await _context.Users.Include(u => u.UserPreferences)
                                               .ThenInclude(up => up.Preference)
                                               .Include(u => u.UserRestrictions)
                                               .ThenInclude(ur => ur.Restriction)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();

                var preferences = await _context.Preferences.ToListAsync();
                var restrictions = await _context.Restrictions.ToListAsync();

                return Ok(new PublicUser(user, null, preferences, restrictions));
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Updates the public profile of a user based on their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="model">The <c>PublicUser</c> model containing the updated data.</param>
        /// <returns>
        /// An <c>ActionResult</c> containing the updated <c>PublicUser</c> object or an error response.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PublicUser>> Edit(string id, [FromBody] PublicUser model)
        {
            if (model == null || id != model.Id) return BadRequest("Invalid data provided.");

            try {
                var user = await _context.Users.Include(u => u.UserPreferences)
                                               .Include(u => u.UserRestrictions)
                                               .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return NotFound();

                user.UpdateFromModel(model);

                user.UserPreferences.UpdateCollection(model.Preferences, e => new UserPreference { UserId = user.Id, PreferenceId = e.Id });
                user.UserRestrictions.UpdateCollection(model.Restrictions, e => new UserRestriction { UserId = user.Id, RestrictionId = e.Id });

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new PublicUser(user, model, [], []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}


