using backend.Models.Enums;

namespace backend.Models.Goals
{
    /// <summary>
    /// Represents a log entry for a user's dietary goal actions.
    /// </summary>
    public class GoalLog
    {
        /// <summary>
        /// The unique identifier of the goal associated with the log entry.
        /// </summary>
        public int GoalId { get; set; }

        /// <summary>
        /// The identifier of the user who performed the action related to the goal.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A reference to the user associated with the log entry.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// The type of goal (Automatic or Manual) for this log entry.
        /// </summary>
        public GoalType GoalType { get; set; }

        /// <summary>
        /// The type of automatic goal (Lose Weight, Maintain Weight, or Gain Weight) for this log entry.
        /// Only applicable when <see cref="GoalType"/> is Automatic.
        /// </summary>
        public AutoGoalType? AutoGoalType { get; set; }

        /// <summary>
        /// The target number of calories for this log entry. Defined only for manual goals.
        /// </summary>
        public int? Calories { get; set; }

        /// <summary>
        /// The start date of the goal associated with this log entry.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date of the goal associated with this log entry.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The action performed by the user related to the goal (e.g., "Goal Achieved", "Goal Updated").
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// The date and time when the action was performed.
        /// </summary>
        public DateTime ActionDate { get; set; }
    }
}
