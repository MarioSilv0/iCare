/// <summary>
/// This file defines the <c>PublicUser</c> class, which represents a user's publicly
/// accessible profile, including preferences and restrictions.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>

using backend.Models.Preferences;
using backend.Models.Restrictions;

namespace backend.Models
{
    /// <summary>
    /// Class <c>PublicUser</c> represents a user's public profile with personal details,
    /// preferences, and restrictions.
    /// </summary>
    public class PublicUser
    {
        /// <value>
        /// Property <c>Id</c> represents the unique identifier of the user.
        /// </value>
        public string Id { get; set; }

        /// <value>
        /// Property <c>Picture</c> represents the user's profile picture URL.
        /// </value>
        public string? Picture { get; set; }

        /// <value>
        /// Property <c>Name</c> represents the user's name.
        /// </value>
        public string? Name { get; set; }

        /// <value>
        /// Property <c>Email</c> represents the user's email address.
        /// </value>
        public string? Email { get; set; }

        /// <value>
        /// Property <c>Birthdate</c> represents the user's birthdate.
        /// </value>
        public DateOnly Birthdate { get; set; }

        /// <value>
        /// Property <c>Notifications</c> indicates whether the user has notifications enabled.
        /// </value>
        public Boolean Notifications { get; set; }

        /// <value>
        /// Property <c>Height</c> represents the user's height in meters.
        /// </value>
        public float Height { get; set; }

        /// <value>
        /// Property <c>Weight</c> represents the user's weight in kilograms.
        /// </value>
        public float Weight { get; set; }

        /// <value>
        /// Property <c>Preferences</c> contains a list of the user's preferences.
        /// </value>
        public List<SelectionObject> Preferences { get; set; } = new();

        /// <value>
        /// Property <c>Restrictions</c> contains a list of the user's restrictions.
        /// </value>
        public List<SelectionObject> Restrictions { get; set; } = new();

        /// <summary>
        /// Default constructor for the <c>PublicUser</c> class.
        /// </summary>
        public PublicUser() { }

        /// <summary>
        /// Initializes a new <c>PublicUser</c> instance using a <c>User</c> object, an optional
        /// <c>PublicUser</c> model, and lists of preferences and restrictions.
        /// </summary>
        /// <param name="user">The <c>User</c> object containing user details.</param>
        /// <param name="model">
        /// An optional <c>PublicUser</c> instance to provide existing preferences and restrictions.
        /// </param>
        /// <param name="listPreferences">The list of all available preferences.</param>
        /// <param name="listRestrictions">The list of all available restrictions.</param>
        public PublicUser(User user, PublicUser? model, List<Preference> listPreferences, List<Restriction> listRestrictions)
        {
            List<SelectionObject> preferences = (model != null) ? model.Preferences : GetSelectionList(listPreferences, e => e.Id, e => e.Name, id => user.UserPreferences.Any(up => up.PreferenceId == id));
            List<SelectionObject> restrictions = (model != null) ? model.Restrictions : GetSelectionList(listRestrictions, e => e.Id, e => e.Name, id => user.UserRestrictions.Any(up => up.RestrictionId == id));

            Id = user.Id;
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

        /// <summary>
        /// Generates a selection list from a given list of items, using provided
        /// selectors and logic to determine selection status.
        /// </summary>
        /// <typeparam name="T">The type of elements in the input list.</typeparam>
        /// <param name="list">The list of items to transform into a selection list.</param>
        /// <param name="idSelector">A function to select the ID from an item.</param>
        /// <param name="nameSelector">A function to select the name from an item.</param>
        /// <param name="isSelectedLogic">A function to determine if an item is selected.</param>
        /// <returns>A list of <c>SelectionObject</c> instances.</returns>
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
