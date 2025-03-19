namespace backend.Models.Enums
{
    /// <summary>
    /// Represents the gender of a user.
    /// </summary>
    /// <author>Mário Silva - 202000500</author>
    /// <date>Last Modified: 2025-03-17</date> 
    public enum Gender
    {
        Male,
        Female
    }

    public static class GenderExtensions
    {
        /// <summary>
        /// Converts a string to a <see cref="Gender"/> enum value, defaulting to Male if invalid.
        /// </summary>
        /// <param name="gender">The string representing the gender.</param>
        /// <returns>The corresponding <see cref="Gender"/>, or Male if invalid.</returns>
        public static Gender FromString(string? gender) =>
            Enum.TryParse(gender, true, out Gender result) ? result : Gender.Male;

        /// <summary>
        /// Converts a <see cref="Gender"/> enum value to a user-friendly string.
        /// </summary>
        /// <param name="gender">The <see cref="Gender"/> value.</param>
        /// <returns>The string representation of the gender.</returns>
        public static string ToFriendlyString(this Gender? gender) =>
            gender?.ToString() ?? "Male";
    }
}
