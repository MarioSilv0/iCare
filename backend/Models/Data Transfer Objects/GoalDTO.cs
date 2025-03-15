using backend.Models.Goals;
using backend.Models.Ingredients;

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
        public required string GoalType { get; set; }

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
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of the goal. This field is optional.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Default constructor for the <c>IngredientDTO</c> class.
        /// </summary>
        public GoalDTO() { }

        /// <summary>
        /// Initializes a new instance of the <c>IngredientDTO</c> class using an <see cref="Ingredient"/> object.
        /// This constructor extracts relevant ingredient details for data transfer.
        /// </summary>
        /// <param name="ingredient">The <see cref="Ingredient"/> entity containing ingredient details.</param>
        public GoalDTO(Goal goal)
        {

        }
    }
}
