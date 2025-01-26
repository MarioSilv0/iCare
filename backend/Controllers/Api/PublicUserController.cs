using backend.Data;
using backend.Models;
using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PublicUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublicUser>> Edit(string id)
        {
            var user = await _context.Users.Include(u => u.UserPreferences)
                                           .ThenInclude(up => up.Preference)
                                           .Include(u => u.UserRestrictions) 
                                           .ThenInclude(ur => ur.Restriction)
                                           .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var preferences = await _context.Preferences.ToListAsync();
            var restrictions = await _context.Restrictions.ToListAsync();
            var publicUser = new PublicUser
            {
                Picture = user.Picture,
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Notifications = user.Notifications,
                Height = user.Height,
                Weight = user.Weight,
                Preferences = preferences.Select(p => new SelectionObject
                {
                    Id = p.Id,
                    Name = p.Name,
                    IsSelected = user.UserPreferences.Any(up => up.PreferenceId == p.Id),
                }).ToList(),
                Restrictions = restrictions.Select(r => new SelectionObject
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = user.UserRestrictions.Any(ur => ur.RestrictionId == r.Id),
                }).ToList(),
            };

            return publicUser;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PublicUser>> Edit(string id, [FromBody] PublicUser model)
        {
            if (model == null || id != model.Id)
            {
                return BadRequest();
            }

            var user = await _context.Users.Include(u => u.UserPreferences)
                                           .Include(u => u.UserRestrictions)
                                           .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Picture = string.IsNullOrEmpty(model.Picture)? user.Picture : model.Picture;
            user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
            user.Email = string.IsNullOrEmpty(model.Email) ? user.Email : model.Email;
            user.Birthdate = model.Birthdate;
            user.Notifications = model.Notifications;
            user.Height = model.Height <= 0 || model.Height > 3 ? user.Height : model.Height;
            user.Weight = model.Weight <= 0 || model.Weight > 700 ? user.Weight : model.Weight;

            user.UserPreferences.Clear();
            foreach(SelectionObject p in model.Preferences)
            {
                if (!p.IsSelected) continue;

                user.UserPreferences.Add(new UserPreference
                {
                    UserId = user.Id,
                    PreferenceId = p.Id
                });
            }

            user.UserRestrictions.Clear();
            foreach (SelectionObject r in model.Restrictions)
            {
                if (!r.IsSelected) continue;

                user.UserRestrictions.Add(new UserRestriction
                {
                    UserId = user.Id,
                    RestrictionId = r.Id
                });
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            PublicUser pu = new PublicUser
            {
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Height = user.Height,
                Weight = user.Weight,
                Preferences = model.Preferences
            };

            return pu;
        }
    }

}


