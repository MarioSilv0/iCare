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

}
