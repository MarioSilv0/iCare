using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    /// <summary>
    /// Provides functionality for managing user goals, including creating, updating, retrieving, and deleting goals.
    /// This service validates goal data, supports automatic and manual goal types, and calculates caloric goals based on user profiles.
    /// </summary>
    public class GoalService : IGoalService
    {
        private readonly ICareServerContext _context;
        private const int MinCalories = 1200;
        private const int MaxCalories = 4000;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoalService"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The database context to interact with.</param>
        public GoalService(ICareServerContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the latest goal for a specified user by their user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose latest goal is to be retrieved.</param>
        /// <returns>The most recent goal for the user, or <c>null</c> if no goal exists.</returns>
        public async Task<Goal?> GetLatestGoalByUserIdAsync(string userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.StartDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new goal for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the goal is being created.</param>
        /// <param name="goalDto">The data transfer object containing the goal details.</param>
        /// <returns>The created <see cref="Goal"/> object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal validation fails.</exception>
        public async Task<Goal> CreateGoalAsync(string userId, GoalDTO goalDto)
        {
            var goal = new Goal
            {
                UserId = userId,
                GoalType = GoalTypeExtensions.FromString(goalDto.GoalType),
                AutoGoalType = AutoGoalTypeExtensions.FromString(goalDto.AutoGoalType),
                Calories = goalDto.Calories,
                StartDate = goalDto.StartDate ?? DateTime.UtcNow,
                EndDate = goalDto.EndDate ?? DateTime.UtcNow.AddMonths(1)
            };

            var (Success, ErrorMessage) = ValidateGoal(goal);
            if (!Success)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            if (goal.GoalType == GoalType.Automatica)
            {
                var user = await _context.Users.FindAsync(userId);
                goal.Calories = CalculateAutomaticGoal(user, goal.AutoGoalType);
            }

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return goal;
        }

        /// <summary>
        /// Updates an existing goal for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose goal is to be updated.</param>
        /// <param name="id">The ID of the goal to be updated.</param>
        /// <param name="goalDto">The data transfer object containing the updated goal details.</param>
        /// <returns><c>true</c> if the goal was successfully updated, otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal validation fails.</exception>
        public async Task<bool> UpdateGoalAsync(string userId, int id, GoalDTO goalDto)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return false;
            }

            goal.GoalType = GoalTypeExtensions.FromString(goalDto.GoalType);
            goal.AutoGoalType = AutoGoalTypeExtensions.FromString(goalDto.AutoGoalType);
            goal.Calories = goalDto.Calories;
            goal.StartDate = goalDto.StartDate ?? DateTime.UtcNow;
            goal.EndDate = goalDto.EndDate ?? DateTime.UtcNow.AddMonths(1);

            var (Success, ErrorMessage) = ValidateGoal(goal);
            if (!Success)
            {
                throw new InvalidOperationException(ErrorMessage);
            }

            if (goal.GoalType == GoalType.Automatica)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                goal.Calories = CalculateAutomaticGoal(user!, goal.AutoGoalType);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes a goal for a specified user.
        /// </summary>
        /// <param name="id">The ID of the goal to be deleted.</param>
        /// <returns><c>true</c> if the goal was successfully deleted, otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteGoalAsync(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return false;
            }

            var goalLog = new GoalLog
            {
                GoalId = goal.Id,
                UserId = goal.UserId,
                Calories = goal.Calories,
                GoalType = goal.GoalType,
                AutoGoalType = goal.AutoGoalType,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Action = "Deleted",
                ActionDate = DateTime.UtcNow
            };

            _context.GoalLogs.Add(goalLog);
            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Validates the specified goal for correctness.
        /// </summary>
        /// <param name="goal">The goal to be validated.</param>
        /// <returns>A tuple containing a boolean indicating success and an error message if validation fails.</returns>
        public (bool Success, string? ErrorMessage) ValidateGoal(Goal goal)
        {
            if (goal.GoalType == GoalType.Automatica && !goal.AutoGoalType.HasValue)
            {
                return (false, "O tipo de meta automática deve ter um valor definido para AutoGoalType.");
            }

            if (goal.GoalType == GoalType.Manual && !goal.Calories.HasValue)
            {
                return (false, "O valor das calorias deve ser definido para metas manuais.");
            }
            else if (goal.Calories < MinCalories || goal.Calories > MaxCalories)
            {
                return (false, "Valor calórico deve estar entre 1200 e 4000 kcal.");
            }

            if (goal.StartDate >= goal.EndDate)
            {
                return (false, "A data de início deve ser anterior à data de término.");
            }

            return (true, null);
        }

        /// <summary>
        /// Calculates the automatic caloric goal based on a user's profile and activity level.
        /// </summary>
        /// <param name="profile">The user's profile containing weight, height, age, and activity level.</param>
        /// <param name="autoGoalType">The type of automatic goal (lose weight, maintain weight, gain weight).</param>
        /// <returns>The calculated caloric goal based on the user's profile and goal type.</returns>
        private int CalculateAutomaticGoal(User profile, AutoGoalType? autoGoalType)
        {
            double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age() + (profile.Gender.Equals("Male") ? 5 : -161);

            double activityMultiplier = profile.ActivityLevel switch
            {
                ActivityLevel.Sedentary => 1.2,
                ActivityLevel.LightlyActive => 1.375,
                ActivityLevel.ModeratelyActive => 1.55,
                ActivityLevel.VeryActive => 1.725,
                ActivityLevel.SuperActive => 1.9,
                _ => 1.2
            };

            int maintenanceCalories = (int)(bmr * activityMultiplier);

            return autoGoalType switch
            {
                AutoGoalType.PerderPeso => maintenanceCalories - 500,
                AutoGoalType.ManterPeso => maintenanceCalories,
                AutoGoalType.GanharPeso => maintenanceCalories + 500,
                _ => maintenanceCalories
            };
        }
    }
}
