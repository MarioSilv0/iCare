using backend.Data;
using backend.Models;
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

            if (user == null) return NotFound();

            return await PublicUser.CreatePublicUser(user, null, _context.Preferences.ToListAsync(), _context.Restrictions.ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PublicUser>> Edit(string id, [FromBody] PublicUser model)
        {
            if (model == null || id != model.Id) return BadRequest();

            var user = await _context.Users.Include(u => u.UserPreferences)
                                           .Include(u => u.UserRestrictions)
                                           .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            user.Picture = string.IsNullOrEmpty(model.Picture) ? user.Picture : model.Picture;
            user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
            user.Email = string.IsNullOrEmpty(model.Email) ? user.Email : model.Email;
            user.Birthdate = model.Birthdate;
            user.Notifications = model.Notifications;
            user.Height = model.Height <= 0 || model.Height > 3 ? user.Height : model.Height;
            user.Weight = model.Weight <= 0 || model.Weight > 700 ? user.Weight : model.Weight;

            UpdateCollection(user.UserPreferences, model.Preferences, e => new UserPreference { UserId = user.Id, PreferenceId = e.Id });
            UpdateCollection(user.UserRestrictions, model.Restrictions, e => new UserRestriction { UserId = user.Id, RestrictionId = e.Id });

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return await PublicUser.CreatePublicUser(user, model, _context.Preferences.ToListAsync(), _context.Restrictions.ToListAsync());
        }

        private void UpdateCollection<T>(ICollection<T> collection, List<SelectionObject> list, Func<SelectionObject, T> createElement) 
        {
            collection.Clear();
            foreach(SelectionObject e in list)
            {
                if (!e.IsSelected) continue;

                collection.Add(createElement(e));
            }    
        }
    }
}


