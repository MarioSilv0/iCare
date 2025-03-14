using backend.Models.Enums;

namespace backend.Models.Goals
{
    /// <summary>
    /// Represents a dietary goal associated with a user.
    /// </summary>
    public class Goal
    {
        /// <summary>
        /// The unique identifier of the goal.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The identifier of the user to whom the goal belongs.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// The type of the goal (Automatic or Manual).
        /// </summary>
        public required GoalType GoalType { get; set; }

        /// <summary>
        /// The type of automatic goal (Lose Weight, Maintain Weight, or Gain Weight).
        /// Only applicable when <see cref="GoalType"/> is Automatic.
        /// </summary>
        public AutoGoalType? AutoGoalType { get; set; }

        /// <summary>
        /// The target number of calories. Defined only for manual goals.
        /// </summary>
        public int? Calories { get; set; }

        /// <summary>
        /// The start date of the goal.
        /// </summary>
        public required DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the goal.
        /// </summary>
        public required DateTime EndDate { get; set; }

        /// <summary>
        /// The creation date of the goal.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// A reference to the user associated with this goal.
        /// </summary>
        public User? User { get; set; }
    }
}
