namespace backend.Models.Enums
{
    /// <summary>
    /// Represents the biological gender of the user.
    /// </summary>
    public enum Gender
    {
        Male,   // Male gender
        Female, // Female gender
    }

    /// <summary>
    /// Extension methods for the <see cref="Gender"/> enum.
    /// </summary>
    public static class GenderExtensions
    {
        /// <summary>
        /// Converts a string value to a corresponding <see cref="Gender"/>.
        /// </summary>
        /// <param name="gender">The string value representing the gender.</param>
        /// <returns>The corresponding <see cref="Gender"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the string value is invalid.</exception>
        public static Gender FromString(string? gender)
        {
            return gender switch
            {
                "Male" => Gender.Male,
                "Female" => Gender.Female,
                _ => Gender.Male,
            };
        }

        /// <summary>
        /// Converts a <see cref="Gender"/> to its string representation.
        /// </summary>
        /// <param name="gender">The <see cref="Gender"/> to convert.</param>
        /// <returns>The string representation of the <see cref="Gender"/>.</returns>
        public static string ToFriendlyString(this Gender? gender)
        {
            return gender switch
            {
                Gender.Male => "Male",
                Gender.Female => "Female",
                _ => "Male",
            };
        }
    }
}
