using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Services
{
    public class GoalService : IGoalService
    {
        private readonly ICareServerContext _context;

        public GoalService(ICareServerContext context)
        {
            _context = context;
        }

        public async Task<Goal?> GetLatestGoalByUserIdAsync(string userId)
        {
            var goal = await _context.Goals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.StartDate)
                .FirstOrDefaultAsync();

            if (goal != null && goal.EndDate <= DateTime.UtcNow){
                await DeleteGoalAsync(userId);
                return null;
            }
            return goal;
        }

        public async Task<Goal> CreateGoalAsync(string userId, GoalDTO goalDto)
        {
            var goal = new Goal
            {
                UserId = userId,
                GoalType = GoalTypeExtensions.FromString(goalDto.GoalType),
                AutoGoalType = AutoGoalTypeExtensions.FromString(goalDto.AutoGoalType),
                Calories = goalDto.Calories,
                StartDate = goalDto.StartDate.ToDateTime(TimeOnly.MinValue),
                EndDate = goalDto.EndDate.ToDateTime(TimeOnly.MinValue)
            };
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId) ?? throw new InvalidOperationException("User not found");
            var (Success, ErrorMessage) = ValidateAndCalculateGoal(user, goal);
            if (!Success)
                throw new InvalidOperationException(ErrorMessage);

            _context.Goals.Add(goal);
            
            var goalLog = new GoalLog
            {
                GoalId = goal.Id,
                UserId = goal.UserId,
                Calories = goal.Calories,
                GoalType = goal.GoalType,
                AutoGoalType = goal.AutoGoalType,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Action = "Created",
                ActionDate = DateTime.UtcNow
            };

            _context.GoalLogs.Add(goalLog);
            await _context.SaveChangesAsync();


            return goal;
        }

        public async Task<bool> UpdateGoalAsync(string userId, GoalDTO goalDto)
        {
            var goal = await GetLatestGoalByUserIdAsync(userId);
            if (goal == null)
                return false;
            

            goal.GoalType = GoalTypeExtensions.FromString(goalDto.GoalType);
            goal.AutoGoalType = AutoGoalTypeExtensions.FromString(goalDto.AutoGoalType);
            goal.Calories = goalDto.Calories;
            goal.StartDate = goalDto.StartDate.ToDateTime(TimeOnly.MinValue);
            goal.EndDate = goalDto.EndDate.ToDateTime(TimeOnly.MinValue);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var (Success, ErrorMessage) = ValidateAndCalculateGoal(user, goal);
            if (!Success)
                throw new InvalidOperationException(ErrorMessage);

            var goalLog = new GoalLog
            {
                GoalId = goal.Id,
                UserId = goal.UserId,
                Calories = goal.Calories,
                GoalType = goal.GoalType,
                AutoGoalType = goal.AutoGoalType,
                StartDate = goal.StartDate,
                EndDate = goal.EndDate,
                Action = "Updated",
                ActionDate = DateTime.UtcNow
            };

            _context.GoalLogs.Add(goalLog);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteGoalAsync(string userId)
        {
            var goal = await GetLatestGoalByUserIdAsync(userId);
            if (goal == null)
                return false;

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

        public (bool Success, string? ErrorMessage) ValidateAndCalculateGoal(User user, Goal goal)
        {
            if (goal.GoalType == GoalType.Automatica)
            {
                if (!goal.AutoGoalType.HasValue)
                    return (false, "O tipo de meta automática deve ter um valor definido para AutoGoalType.");
                else
                    goal.Calories = CalculateAutomaticGoal(user!, goal.AutoGoalType);
            }
            else
            {
                if (!goal.Calories.HasValue)
                    return (false, "O valor das calorias deve ser definido para metas manuais.");
            }

            if (goal.StartDate > goal.EndDate)
            {
                return (false, "A data de início deve ser anterior à data de término.");
            }

            return (true, null);
        }

        public int CalculateAutomaticGoal(User user, AutoGoalType? autoGoalType)
        {
            double bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age() + (user.Gender.Equals(Gender.Male) ? 5 : -161);

            double activityMultiplier = user.ActivityLevel switch
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
