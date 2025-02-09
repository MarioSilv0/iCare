using backend.Models.Preferences;
using backend.Models.Restrictions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

/// <summary>
/// Represents an application user with personal details, preferences, and restrictions.
/// Inherits from <see cref="IdentityUser"/> to provide authentication and identity features.
/// </summary>
/// <author>Luís Martins - 202100239</author>
/// <author>João Morais - 202001541</author>
/// <date>Last Modified: 2025-01-27</date>
namespace backend.Models
{
    /// <summary>
    /// Represents a user in the system, inheriting from <see cref="IdentityUser"/>.
    /// This model includes additional properties such as personal details, preferences, and restrictions.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        [Display(Name = "Name")]
        [PersonalData]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the profile picture URL of the user.
        /// </summary>
        [Display(Name = "Picture")]
        public string? Picture { get; set; }

        /// <summary>
        /// Gets or sets the user's birthdate.
        /// </summary>
        [Display(Name = "Birthdate")]
        public DateOnly Birthdate { get; set; } = new DateOnly();

        /// <summary>
        /// Indicates whether the user has enabled notifications.
        /// </summary>
        [Display(Name = "Notifications")]
        public bool Notifications { get; set; } = false;

        /// <summary>
        /// Gets or sets the user's height in meters.
        /// </summary>
        [Display(Name = "Height")]
        public float Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the user's weight in kilograms.
        /// </summary>
        [Display(Name = "Weight")]
        public float Weight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the user's preferences.
        /// </summary>
        [Display(Name = "Preferences")]
        public ICollection<UserPreference>? UserPreferences { get; set; }

        /// <summary>
        /// Gets or sets the user's restrictions.
        /// </summary>
        [Display(Name = "Restrictions")]
        public ICollection<UserRestriction>? UserRestrictions { get; set; }

        /// <summary>
        /// Gets or sets the logs associated with the user.
        /// </summary>
        public ICollection<UserLog>? Logs { get; set; }

        /// <summary>
        /// Updates the user properties based on the provided <see cref="PublicUser"/> model.
        /// </summary>
        /// <param name="model">The public user model containing updated user data.</param>
        public void UpdateFromModel(PublicUser model)
        {
            if(Picture != model.Picture && !string.IsNullOrWhiteSpace(model.Picture))
                Picture = model.Picture;

            if(Name != model.Name && !string.IsNullOrWhiteSpace(model.Name))
                Name = model.Name;

            if(Email != model.Email && !string.IsNullOrWhiteSpace(model.Email))
                Email = model.Email;

            if (Birthdate != model.Birthdate)
            {
                int age = this.CaculateAge(model.Birthdate);
                if (age > 0 && age <= 120)
                    Birthdate = model.Birthdate;
            }

            Notifications = model.Notifications;

            float roundedHeight = (float) Math.Round(model.Height, 1);
            if (Height != roundedHeight && roundedHeight > 0 && roundedHeight < 3)
                Height = roundedHeight;

            float roundedWeight = (float) Math.Round(model.Weight, 1);
            if (Weight != roundedWeight && roundedWeight > 0 && roundedWeight < 700)
                Weight = roundedWeight;
        }

        /// <summary>
        /// Calculates the age based on the given birthdate.
        /// </summary>
        /// <param name="birthdate">The birthdate of the user.</param>
        /// <returns>The calculated age.</returns>
        private int CaculateAge(DateOnly birthdate)
        {
            int age = DateTime.Today.Year - birthdate.Year;

            // Adjust if birthday hasn't occurred yet
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (birthdate > today.AddYears(-age))
                age -= 1;

            return age;
        }
    }
}


