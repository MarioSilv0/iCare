using backend.Data;
using backend.Models;
using backend.Models.Goals;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class GoalService
    {
        private readonly ICareServerContext _context;
        private readonly User _user;

        public GoalService(ICareServerContext context, User user)
        {
            _context = context;
            _user = user;
        }

        public async Task<Goal?> GetCurrentGoalAsync(string userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .OrderByDescending(g => g.StartDate)
                .FirstOrDefaultAsync();
        }


        public async Task<(bool Success, string? ErrorMessage, Goal? ValidGoal)> ValidateAndSaveGoalAsync(Goal goal, User profile)
        {
            // Validação de Meta Automática
            if (goal.Type == "Automática")
            {
                goal.Calories = CalculateAutomaticGoal(profile, goal.GoalType);
            }
            else if (goal.Type == "Manual")
            {
                var validationMessage = ValidateManualGoal(goal);
                if (validationMessage != null)
                {
                    return (false, validationMessage, null);
                }
            }

            // Verifica se já existe uma meta ativa para o utilizador
            var existingGoal = await _context.Goals
                .Where(g => g.UserId == goal.UserId)
                .OrderByDescending(g => g.StartDate)
                .FirstOrDefaultAsync();

            if (existingGoal != null)
            {
                return (false, "Utilizador já tem uma meta ativa.", null);
            }

            // Guarda a meta válida na base de dados
            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return (true, null, goal);
        }

        private int CalculateAutomaticGoal(User profile, string? goalType)
        {
            double bmr = 10 * profile.Weight + 6.25 * profile.Height - 5 * CalculateAge(profile.Birthdate) + (profile.Gender == "M" ? 5 : -161);
            double activityMultiplier = profile.ActivityLevel switch
            {
                "Sedentário" => 1.2,
                "Levemente Ativo" => 1.375,
                "Moderadamente Ativo" => 1.55,
                "Muito Ativo" => 1.725,
                _ => 1.2
            };

            int maintenanceCalories = (int)(bmr * activityMultiplier);

            return goalType switch
            {
                "Perder-Peso" => maintenanceCalories - 500, // Déficit de 500 kcal/dia
                "Manter-Peso" => maintenanceCalories,
                "Ganhar-Peso" => maintenanceCalories + 500, // Excesso de 500 kcal/dia
                _ => maintenanceCalories
            };
        }

        private string? ValidateManualGoal(Goal goal)
        {
            if (goal.Calories < 1200 || goal.Calories > 4000)
            {
                return "Valor calórico deve estar entre 1200 e 4000 kcal.";
            }

            if (goal.StartDate >= goal.EndDate)
            {
                return "A data de início deve ser anterior à data de término.";
            }

            return null;
        }
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }





        public async Task<bool> UpdateGoalAsync(Goal goal)
        {
            var existingGoal = await _context.Goals.FindAsync(goal.Id);
            if (existingGoal == null) return false;

            // Atualiza apenas os campos modificáveis
            existingGoal.Type = goal.Type;
            existingGoal.GoalType = goal.GoalType;
            existingGoal.Calories = goal.Calories;
            existingGoal.StartDate = goal.StartDate;
            existingGoal.EndDate = goal.EndDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteGoalAsync(int id)
        {
            var goal = await _context.Goals.FindAsync(id);
            if (goal == null) return false;

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
