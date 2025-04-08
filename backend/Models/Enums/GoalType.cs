namespace backend.Models.Enums
{
    /// <summary>
    /// Represents the type of goal, either automatic or manual.
    /// </summary>
    /// <author>Mário Silva - 202000500</author>
    /// <date>Last Modified: 2025-03-17</date> 
    public enum GoalType
    {
        /// <summary>
        /// Automatic goal type.
        /// </summary>
        Automatica,  // "Automática"

        /// <summary>
        /// Manual goal type.
        /// </summary>
        Manual       // "Manual"
    }

    /// <summary>
    /// Represents the type of automatic goal (Lose Weight, Maintain Weight, Gain Weight).
    /// </summary>
    public enum AutoGoalType
    {
        /// <summary>
        /// Goal to lose weight.
        /// </summary>
        PerderPeso,  // "Perder-Peso"

        /// <summary>
        /// Goal to maintain current weight.
        /// </summary>
        ManterPeso,  // "Manter-Peso"

        /// <summary>
        /// Goal to gain weight.
        /// </summary>
        GanharPeso   // "Ganhar-Peso"
    }

    /// <summary>
    /// Extension methods for the <see cref="GoalType"/> enum.
    /// </summary>

    public static class GoalTypeExtensions
    {
        /// <summary>
        /// Converts a string to a corresponding <see cref="GoalType"/> enum value, defaulting to Automatica if invalid.
        /// </summary>
        /// <param name="goalType">The string representing the goal type.</param>
        /// <returns>The corresponding <see cref="GoalType"/>, or Automatica if invalid.</returns>
        public static GoalType FromString(string goalType) =>
            goalType switch
            {
                "Manual" => GoalType.Manual,
                "Automatica" => GoalType.Automatica,
                _ => GoalType.Automatica
            };

        /// <summary>
        /// Converts a <see cref="GoalType"/> enum value to a user-friendly string.
        /// </summary>
        /// <param name="goalType">The <see cref="GoalType"/> value.</param>
        /// <returns>The string representation of the goal type.</returns>
        public static string ToFriendlyString(this GoalType goalType) =>
            goalType.ToString();
    }

    /// <summary>
    /// Extension methods for the <see cref="AutoGoalType"/> enum.
    /// </summary>
    public static class AutoGoalTypeExtensions
    {
        /// <summary>
        /// Converts a string value to a corresponding <see cref="AutoGoalType"/>.
        /// </summary>
        /// <param name="autoGoalType">The string value representing the automatic goal type.</param>
        /// <returns>The corresponding <see cref="AutoGoalType"/>, or null if not found.</returns>
        public static AutoGoalType? FromString(string? autoGoalType)
        {
            return autoGoalType switch
            {
                "Perder Peso" => AutoGoalType.PerderPeso,
                "Manter Peso" => AutoGoalType.ManterPeso,
                "Ganhar Peso" => AutoGoalType.GanharPeso,
                _ => null,
            };
        }

        /// <summary>
        /// Converts an <see cref="AutoGoalType"/> to its string representation.
        /// </summary>
        /// <param name="autoGoalType">The <see cref="AutoGoalType"/> to convert.</param>
        /// <returns>The string representation of the <see cref="AutoGoalType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the <see cref="AutoGoalType"/> is invalid.</exception>
        public static string? ToFriendlyString(this AutoGoalType? autoGoalType)
        {
            return autoGoalType switch
            {
                AutoGoalType.PerderPeso => "Perder Peso",
                AutoGoalType.ManterPeso => "Manter Peso",
                AutoGoalType.GanharPeso => "Ganhar Peso",
                _ => null,
            };
        }
    }
}
