using backend.Data;
using backend.Models;
using backend.Models.Extensions;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PublicUserController> _logger;

        public PublicUserController(ApplicationDbContext context, ILogger<PublicUserController> logger)
        {
            _context = context;
            _logger = logger;
        }

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


