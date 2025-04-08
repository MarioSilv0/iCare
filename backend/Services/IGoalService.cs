using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;

namespace backend.Services
{
    /// <summary>
    /// Defines the contract for goal-related operations, ensuring validation and consistency.
    /// </summary>
    /// <author>Mário Silva - 202000500</author>
    /// <date>Last Modified: 2025-03-14</date>
    public interface IGoalService
    {
        /// <summary>
        /// Retrieves the latest goal for a user by their unique user ID.
        /// The goal is ordered by the start date, with the most recent goal returned.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose latest goal is to be retrieved.</param>
        /// <returns>The latest goal if found; otherwise, null if no goal exists for the user.</returns>
        Task<Goal?> GetLatestGoalByUserIdAsync(string userId);

        /// <summary>
        /// Creates a new dietary goal for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="goalDto">The goal data transfer object containing goal details.</param>
        /// <returns>The created goal.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal is invalid.</exception>
        Task<Goal> CreateGoalAsync(string userId, GoalDTO goalDto);

        /// <summary>
        /// Deletes a goal and logs the action.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteGoalAsync(string userId);

        /// <summary>
        /// Updates an existing goal.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="goalDto">The updated goal data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal is invalid.</exception>
        Task<bool> UpdateGoalAsync(string userId, GoalDTO goalDto);

        /// <summary>
        /// Validates a dietary goal based on predefined rules and calculates calories if necessary.
        /// </summary>
        /// <param name="user">The user for whom the goal is being validated.</param>
        /// <param name="goal">The goal to validate.</param>
        /// <returns>
        /// A tuple indicating:
        /// - Success (bool): Whether the validation passed.
        /// - ErrorMessage (string?): The error message if validation fails.
        /// </returns>
        (bool Success, string? ErrorMessage) ValidateAndCalculateGoal(User user, Goal goal);

        /// <summary>
        /// Calculates the recommended daily caloric intake based on user information and automatic goal type.
        /// Uses the Mifflin-St Jeor formula for BMR calculation and applies an activity level multiplier.
        /// </summary>
        /// <param name="user">The user whose information is used to calculate the goal.</param>
        /// <param name="autoGoalType">The automatic goal type (e.g., Lose Weight, Maintain Weight, Gain Weight).</param>
        /// <returns>
        /// The calculated number of calories to meet the specified automatic goal.
        /// Returns adjusted calories based on the maintenance level and goal type.
        /// </returns>
        int CalculateAutomaticGoal(User user, AutoGoalType? autoGoalType);

    }
}
