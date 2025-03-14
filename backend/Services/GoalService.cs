using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// This file defines the <c>GoalService</c> class, responsible for handling the business logic related to user goals.
/// It provides methods to create, retrieve, update, and delete dietary goals, ensuring validation and consistency.
/// </summary>
/// <author>Mário Silva - 202000500</author>
/// <date>Last Modified: 2025-03-14</date>

namespace backend.Services
{
    public class GoalService
    {
        private readonly ICareServerContext _context;
        private readonly ILogger<GoalService> _logger;
        private const int MinCalories = 1200;
        private const int MaxCalories = 4000;

        public GoalService(ICareServerContext context, ILogger<GoalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a goal by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the goal.</param>
        /// <returns>The goal if found; otherwise, null.</returns>
        public async Task<Goal?> GetGoalByIdAsync(int id)
        {
            return await _context.Goals.FindAsync(id);
        }

        /// <summary>
        /// Creates a new dietary goal for a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="goalDto">The goal data transfer object containing goal details.</param>
        /// <returns>The created goal.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal is invalid.</exception>
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                goal.Calories = CalculateAutomaticGoal(user!, goal.AutoGoalType);
            }

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return goal;
        }

        /// <summary>
        /// Updates an existing goal.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="id">The ID of the goal to update.</param>
        /// <param name="goalDto">The updated goal data.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the goal is invalid.</exception>
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
        /// Deletes a goal and logs the action.
        /// </summary>
        /// <param name="id">The ID of the goal to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
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
        /// Validates a dietary goal based on predefined rules.
        /// </summary>
        /// <param name="goal">The goal to validate.</param>
        /// <returns>
        /// A tuple indicating:
        /// - Success (bool): Whether the validation passed.
        /// - ErrorMessage (string?): The error message if validation fails.
        /// </returns>
        private (bool Success, string? ErrorMessage) ValidateGoal(Goal goal)
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
        /// Calculates the recommended daily calorie intake based on the user's profile and goal type.
        /// </summary>
        /// <param name="profile">The user's profile containing physical characteristics.</param>
        /// <param name="autoGoalType">The selected auto goal type (e.g., lose, maintain, or gain weight).</param>
        /// <returns>The recommended daily calorie intake.</returns>
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
