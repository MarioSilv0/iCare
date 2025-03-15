using backend.Models.Enums;

/// <summary>
/// This file defines the <c>UserPhysicalDTO</c> class, which represents a user's
/// physical attributes for data transfer purposes.
/// </summary>
/// <author>Mário Silva  - 202000500</author>
/// <date>Last Modified: 2025-03-14</date>

namespace backend.Models
{
    /// <summary>
    /// The <c>UserPhysicalDTO</c> class represents a user's physical attributes,
    /// including height, weight, birthdate, gender, and activity level.
    /// This class is used for data transfer where only physical information is needed.
    /// </summary>
    public class UserPhysicalDTO
    {
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
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's activity level.
        /// </summary>
        public ActivityLevel ActivityLevel { get; set; }

        /// <summary>
        /// Initializes a new instance of the <c>UserPhysicalDTO</c> class.
        /// </summary>
        public UserPhysicalDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>UserPhysicalDTO</c> class with data from an existing user.
        /// </summary>
        /// <param name="user">The user entity containing physical attributes.</param>
        public UserPhysicalDTO(User user)
        {
            Birthdate = user.Birthdate;
            Height = user.Height;
            Weight = user.Weight;
            Gender = user.Gender;
            ActivityLevel = user.ActivityLevel;
        }
    }
}
