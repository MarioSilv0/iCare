/// <summary>
/// This file defines the <c>User</c> class, which represents an application user with personal details, 
/// preferences, restrictions, and authentication features.
/// The class extends <see cref="IdentityUser"/> to integrate with ASP.NET Core Identity for user authentication.
/// </summary>
/// <author>João Morais  - 202001541</author>
/// <author>Luís Martins - 202100239</author>
/// <date>Last Modified: 2025-03-01</date>

using backend.Models.Enums;
using backend.Models.Ingredients;
using backend.Models.Recipes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    /// <summary>
    /// The <c>User</c> class represents a user in the system, extending <see cref="IdentityUser"/>.
    /// It includes additional properties such as profile details, dietary preferences, 
    /// and restrictions. Users can also have associated ingredients and favorite recipes.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        [PersonalData]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the profile picture URL of the user.
        /// </summary>
        public string? Picture { get; set; }

        /// <summary>
        /// Gets or sets the user's birthdate.
        /// </summary>
        public DateOnly Birthdate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-16));

        /// <summary>
        /// Gets the user's age based on this birthdate.
        /// </summary>
        /// <returns>Age of the user.</returns>
        public int Age() => CaculateAge(Birthdate);

        /// <summary>
        /// Gets or sets the user's height in meters.
        /// </summary>
        public float Height { get; set; } = 0;

        /// <summary>
        /// Gets or sets the user's weight in kilograms.
        /// </summary>
        public float Weight { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        public Gender Gender { get; set; } = Gender.Male;

        /// <summary>
        /// Gets or sets the user's activity level.
        /// </summary>
        public ActivityLevel ActivityLevel { get; set; } = ActivityLevel.ModeratelyActive;

        /// <summary>
        /// Indicates whether the user has enabled notifications.
        /// </summary>
        public bool Notifications { get; set; } = false;

        /// <summary>
        /// Gets or sets the user's dietary preferences.
        /// </summary>
        public IList<string>? Preferences { get; set; }

        /// <summary>
        /// Gets or sets the user's dietary restrictions.
        /// </summary>
        public IList<string>? Restrictions { get; set; }

        /// <summary>
        /// Gets or sets the ingredients associated with the user.
        /// </summary>
        public ICollection<UserIngredient>? UserIngredients { get; set; }

        /// <summary>
        /// Gets or sets the user's favorite recipes.
        /// </summary>
        public ICollection<UserRecipe>? FavoriteRecipes { get; set; }

        /// <summary>
        /// Gets or sets the logs associated with the user.
        /// </summary>
        public ICollection<UserLog>? Logs { get; set; }

        /// <summary>
        /// Updates the user properties based on the provided <see cref="UserDTO"/> model.
        /// </summary>
        /// <param name="model">The public user model containing updated user data.</param>
        public void UpdateFromModel(UserDTO model)
        {
            if (Picture != model.Picture && !string.IsNullOrWhiteSpace(model.Picture))
                Picture = model.Picture;

            if (Name != model.Name && !string.IsNullOrWhiteSpace(model.Name))
                Name = model.Name;

            if (Email != model.Email && !string.IsNullOrWhiteSpace(model.Email) && this.IsValidEmail(model.Email))
                Email = model.Email;

            if (Birthdate != model.Birthdate)
            {
                int age = this.CaculateAge(model.Birthdate);
                if (age > 0 && age <= 120)
                    Birthdate = model.Birthdate;
            }

            Gender = GenderExtensions.FromString(model.Gender);
            ActivityLevel = ActivityLevelExtensions.FromString(model.ActivityLevel);

            Notifications = model.Notifications;

            float roundedHeight = (float)Math.Round(model.Height, 2);
            if (Height != roundedHeight && roundedHeight > 0 && roundedHeight < 3)
                Height = roundedHeight;

            float roundedWeight = (float)Math.Round(model.Weight, 2);
            if (Weight != roundedWeight && roundedWeight > 0 && roundedWeight < 700)
                Weight = roundedWeight;

            if (model.Preferences != Preferences)
                Preferences = model.Preferences;

            if (model.Restrictions != Restrictions)
                Restrictions = model.Restrictions;
        }

        /// <summary>
        /// Calculates the user's age based on the given birthdate.
        /// </summary>
        /// <param name="birthdate">The user's birthdate.</param>
        /// <returns>The calculated age of the user.</returns>
        private int CaculateAge(DateOnly birthdate)
        {
            int age = DateTime.Today.Year - birthdate.Year;

            // Adjust if birthday hasn't occurred yet
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (birthdate > today.AddYears(-age))
                age -= 1;

            return age;
        }

        /// <summary>
        /// Validates whether the provided email address follows a valid format.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns><c>true</c> if the email format is valid; otherwise, <c>false</c>.</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}


