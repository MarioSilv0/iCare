
using backend.Models.Enums;

/// <summary>
/// This file defines the <c>UserDTO</c> class, which represents a user's publicly
/// accessible profile, including preferences and restrictions.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-01</date>

namespace backend.Models
{
    /// <summary>
    /// The <c>UserDTO</c> class represents a user's public profile, including personal details,
    /// preferences, and dietary restrictions. This class is used for data transfer where only
    /// publicly available user information is needed.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Gets or sets the URL of the user's profile picture.
        /// </summary>
        public string? Picture { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user's birthdate.
        /// </summary>
        public DateOnly Birthdate { get; set; }

        /// <summary>
        /// Gets or sets the user's height in meters.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gets or sets the user's weight in kilograms.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's activity level.
        /// </summary>
        public string? ActivityLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has notifications enabled.
        /// </summary>
        public Boolean Notifications { get; set; }

        /// <summary>
        /// Gets or sets the user's preferred food categories.
        /// </summary>
        public List<string> Preferences { get; set; } = new();

        /// <summary>
        /// Gets or sets the user's dietary restrictions.
        /// </summary>
        public List<string> Restrictions { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of available food categories.
        /// </summary>
        public List<string> Categories { get; set; } = new();

        /// <summary>
        /// Initializes a new instance of the <c>UserDTO</c> class.
        /// </summary>
        public UserDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>UserDTO</c> class with data from an existing user.
        /// </summary>
        /// <param name="user">The user entity containing profile information.</param>
        /// <param name="model">An optional existing <c>UserDTO</c> model to retrieve preferences and restrictions from.</param>
        /// <param name="categories">A list of available food categories.</param>
        public UserDTO(User user, UserDTO? model, List<string> categories)
        {
            Picture = user.Picture;
            Name = user.Name;
            Email = user.Email;
            Birthdate = user.Birthdate;
            Height = user.Height;
            Weight = user.Weight;
            Gender = GenderExtensions.ToFriendlyString(user.Gender);
            ActivityLevel = ActivityLevelExtensions.ToFriendlyString(user.ActivityLevel);
            Notifications = user.Notifications;
            Preferences = model?.Preferences ?? new List<string>(user.Preferences ?? []);
            Restrictions = model?.Restrictions ?? new List<string>(user.Restrictions ?? []);
            Categories = categories;
        }
    }
}
