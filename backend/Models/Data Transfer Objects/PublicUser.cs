/// <summary>
/// This file defines the <c>PublicUser</c> class, which represents a user's publicly
/// accessible profile, including preferences and restrictions.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais  - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>
namespace backend.Models
{
    /// <summary>
    /// Class <c>PublicUser</c> represents a user's public profile with personal details,
    /// preferences, and restrictions.
    /// </summary>
    public class PublicUser
    {
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

        
        public List<string> Preferences { get; set; } = new();
        public List<string> Restrictions { get; set; } = new();
        public List<string> Categories { get; set; } = new();

        /// <summary>
        /// Default constructor for the <c>PublicUser</c> class.
        /// </summary>
        public PublicUser() { }

        public PublicUser(User user, PublicUser? model, List<string> categories)
        {
            Picture = user.Picture;
            Name = user.Name;
            Email = user.Email;
            Birthdate = user.Birthdate;
            Notifications = user.Notifications;
            Height = user.Height;
            Weight = user.Weight;
            Preferences = model?.Preferences ?? new List<string>(user.Preferences ?? []);
            Restrictions = model?.Restrictions ?? new List<string>(user.Restrictions ?? []);
            Categories = categories;
        }
    }
}
