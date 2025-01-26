using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    public class PublicUser
    {
        public string Id { get; set; }
        public string? Picture { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateOnly Birthdate { get; set; }
        public Boolean Notifications { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public List<SelectionObject> Preferences { get; set; } = new();
        public List<SelectionObject> Restrictions { get; set; } = new();

        public static async Task<PublicUser> CreatePublicUser(User user, PublicUser? model, Task<List<Preference>> listPreferences, Task<List<Restriction>> listRestrictions)
        {
            List<SelectionObject> preferences = (model != null) ? model.Preferences : await GetListAsync(listPreferences, p => p.Id, p => p.Name, id => user.UserPreferences.Any(up => up.PreferenceId == id));
            List<SelectionObject> restrictions = (model != null) ? model.Restrictions : await GetListAsync(listRestrictions, r => r.Id, r => r.Name, id => user.UserRestrictions.Any(up => up.RestrictionId == id));

            return new PublicUser
            {
                Picture = user.Picture,
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Notifications = user.Notifications,
                Height = user.Height,
                Weight = user.Weight,
                Preferences = preferences,
                Restrictions = restrictions,
            };
        }

        private static async Task<List<SelectionObject>> GetListAsync<T>(Task<List<T>> task, Func<T, int> idSelector, Func<T, string> nameSelector, Func<int, bool> isSelectedLogic)
        {
            var result = await task;

            return result.Select(e => new SelectionObject
            {
                Id = idSelector(e),
                Name = nameSelector(e),
                IsSelected = isSelectedLogic(idSelector(e)),
            }).ToList();
        }
    }
}
