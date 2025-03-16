namespace backend.Models.Enums
{
    /// <summary>
    /// Represents the user's activity level, which affects calorie calculation.
    /// </summary>
    public enum ActivityLevel
    {
        Sedentary,         // Little or no exercise
        LightlyActive,     // Light exercise (1–3 days per week)
        ModeratelyActive,  // Moderate exercise (3–5 days per week)
        VeryActive,        // Intense exercise (6–7 days per week)
        SuperActive        // Professional athlete or very heavy exercise
    }

    /// <summary>
    /// Extension methods for the <see cref="ActivityLevel"/> enum.
    /// </summary>
    public static class ActivityLevelExtensions
    {
        /// <summary>
        /// Converts a string value to a corresponding <see cref="ActivityLevel"/>.
        /// </summary>
        /// <param name="activityLevel">The string value representing the activity level.</param>
        /// <returns>The corresponding <see cref="ActivityLevel"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the string value is invalid.</exception>
        public static ActivityLevel FromString(string? activityLevel)
        {
            return activityLevel switch
            {
                "Sedentary" => ActivityLevel.Sedentary,
                "Lightly Active" => ActivityLevel.LightlyActive,
                "Moderately Active" => ActivityLevel.ModeratelyActive,
                "Very Active" => ActivityLevel.VeryActive,
                "Super Active" => ActivityLevel.SuperActive,
                _ => ActivityLevel.ModeratelyActive,
            };
        }

        /// <summary>
        /// Converts an <see cref="ActivityLevel"/> to its string representation.
        /// </summary>
        /// <param name="activityLevel">The <see cref="ActivityLevel"/> to convert.</param>
        /// <returns>The string representation of the <see cref="ActivityLevel"/>.</returns>
        public static string ToFriendlyString(this ActivityLevel? activityLevel)
        {
            return activityLevel switch
            {
                ActivityLevel.Sedentary => "Sedentary",
                ActivityLevel.LightlyActive => "Lightly Active",
                ActivityLevel.ModeratelyActive => "Moderately Active",
                ActivityLevel.VeryActive => "Very Active",
                ActivityLevel.SuperActive => "Super Active",
                _ => "Moderately Active",
            };
        }
    }
}