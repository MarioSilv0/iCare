using backend.Models.Enums;
using backend.Models.Goals;
using backend.Models.Ingredients;
using System;

namespace backend.Models.Data_Transfer_Objects
{
    /// <summary>
    /// Represents the data transfer object for a dietary goal.
    /// </summary>
    public class GoalDTO
    {
        /// <summary>
        /// The type of dietary goal. This field is required.
        /// </summary>
        public string GoalType { get; set; }

        /// <summary>
        /// The automatic goal type, if applicable. This field is optional.
        /// </summary>
        public string? AutoGoalType { get; set; }

        /// <summary>
        /// The number of calories associated with the goal. This field is optional.
        /// </summary>
        public int? Calories { get; set; }

        /// <summary>
        /// The start date of the goal. This field is optional.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// The end date of the goal. This field is optional.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Default constructor for the <c>GoalDTO</c> class.
        /// </summary>
        public GoalDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>GoalDTO</c> class using a <see cref="Goal"/> object.
        /// This constructor extracts relevant goal details for data transfer.
        /// </summary>
        /// <param name="goal">The <see cref="Goal"/> entity containing goal details.</param>
        public GoalDTO(Goal goal)
        {
            GoalType = GoalTypeExtensions.ToFriendlyString(goal.GoalType);
            AutoGoalType = AutoGoalTypeExtensions.ToFriendlyString(goal.AutoGoalType);
            Calories = goal.Calories;
            StartDate = DateOnly.FromDateTime(goal.StartDate);
            EndDate = DateOnly.FromDateTime(goal.EndDate);
        }
    }
}
