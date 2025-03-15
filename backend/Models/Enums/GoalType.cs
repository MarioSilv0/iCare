namespace backend.Models.Enums
{
    /// <summary>
    /// Represents the type of goal, either automatic or manual.
    /// </summary>
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
        /// Converts a string value to a corresponding <see cref="GoalType"/>.
        /// </summary>
        /// <param name="goalType">The string value representing the goal type.</param>
        /// <returns>The corresponding <see cref="GoalType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the string value is invalid.</exception>
        public static GoalType FromString(string goalType)
        {
            return goalType switch
            {
                "Manual" => GoalType.Manual,
                "Automatica" => GoalType.Automatica,
                _ => throw new ArgumentException($"Invalid value for GoalType: {goalType}")
            };
        }

        /// <summary>
        /// Converts a <see cref="GoalType"/> to its string representation.
        /// </summary>
        /// <param name="goalType">The <see cref="GoalType"/> to convert.</param>
        /// <returns>The string representation of the <see cref="GoalType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the <see cref="GoalType"/> is invalid.</exception>
        public static string ToFriendlyString(this GoalType goalType)
        {
            return goalType switch
            {
                GoalType.Manual => "Manual",
                GoalType.Automatica => "Automatica",
                _ => throw new ArgumentException($"Invalid value for GoalType: {goalType}")
            };
        }
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
                _ => null
            };
        }

        /// <summary>
        /// Converts an <see cref="AutoGoalType"/> to its string representation.
        /// </summary>
        /// <param name="autoGoalType">The <see cref="AutoGoalType"/> to convert.</param>
        /// <returns>The string representation of the <see cref="AutoGoalType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the <see cref="AutoGoalType"/> is invalid.</exception>
        public static string ToFriendlyString(this AutoGoalType? autoGoalType)
        {
            return autoGoalType switch
            {
                AutoGoalType.PerderPeso => "Perder Peso",
                AutoGoalType.ManterPeso => "Manter Peso",
                AutoGoalType.GanharPeso => "Ganhar Peso",
                _ => throw new ArgumentException($"Invalid value for AutoGoalType: {autoGoalType}")
            };
        }
    }
}
