using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;

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

        public PublicUser() { }
        public PublicUser(User user, PublicUser? model, List<Preference> listPreferences, List<Restriction> listRestrictions)
        {
            List<SelectionObject> preferences = (model != null) ? model.Preferences : GetSelectionList(listPreferences, e => e.Id, e => e.Name, id => user.UserPreferences.Any(up => up.PreferenceId == id));
            List<SelectionObject> restrictions = (model != null) ? model.Restrictions : GetSelectionList(listRestrictions, e => e.Id, e => e.Name, id => user.UserRestrictions.Any(up => up.RestrictionId == id));

            Picture = user.Picture;
            Name = user.Name;
            Email = user.Email;
            Birthdate = user.Birthdate;
            Notifications = user.Notifications;
            Height = user.Height;
            Weight = user.Weight;
            Preferences = preferences;
            Restrictions = restrictions;
            
        }

        private static List<SelectionObject> GetSelectionList<T>(List<T> list, Func<T, int> idSelector, Func<T, string> nameSelector, Func<int, bool> isSelectedLogic)
        {
            return list.Select(e => new SelectionObject
            {
                Id = idSelector(e),
                Name = nameSelector(e),
                IsSelected = isSelectedLogic(idSelector(e)),
            }).ToList();
        }
    }
}
