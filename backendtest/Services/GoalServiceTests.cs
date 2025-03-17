using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace backendtest.Services
{
    public class GoalServiceTests : IClassFixture<ICareContextFixture>, IDisposable
    {
        private readonly ICareServerContext _context;
        private readonly GoalService _goalService;

        public GoalServiceTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            _goalService = new GoalService(_context);
        }

        void IDisposable.Dispose()
        {
            // Limpar os dados do banco de dados entre os testes
            _context.Goals.RemoveRange(_context.Goals);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetLatestGoalByUserIdAsync_ShouldReturnLatestGoal_WhenUserHasGoals()
        {
            // Arrange
            var userId = "user123";
            var goal1 = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Manual,
                Calories = 2500,
                StartDate = DateTime.UtcNow.AddMonths(-2),
                EndDate = DateTime.UtcNow.AddMonths(1)
            };
            var goal2 = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Manual,
                Calories = 2500,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };
            await _context.Goals.AddAsync(goal1);
            await _context.Goals.AddAsync(goal2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _goalService.GetLatestGoalByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result); 
            Assert.Equal(goal2.Id, result.Id);
        }

        [Fact]
        public async Task GetLatestGoalByUserIdAsync_ShouldReturnNull_WhenUserHasNoGoals()
        {
            // Arrange
            var userId = "user123";

            // Act
            var result = await _goalService.GetLatestGoalByUserIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateGoalAsync_ShouldCalculateCaloriesForAutomaticGoal_WhenGoalTypeIsAutomatica()
        {
            // Arrange
            var userId = "user123"; // Mock do ID do usuário
            var user = new User()
            {
                Id = userId,
                Weight = 70,
                Height = 175,
                Gender = Gender.Male,
                ActivityLevel = ActivityLevel.ModeratelyActive,
                Birthdate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-25))
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var goalDto = new GoalDTO
            {
                GoalType = "Automatica",
                AutoGoalType = "Perder Peso",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
            };

            // Act
            var createdGoal = await _goalService.CreateGoalAsync(userId, goalDto);

            // Assert
            Assert.NotNull(createdGoal);
            Assert.Equal(userId, createdGoal.UserId);
            Assert.Equal(1837, createdGoal.Calories);
        }

        [Fact]
        public async Task CreateGoalAsync_ShouldThrowInvalidOperationException_WhenCaloriesAreOutOfRange()
        {
            // Arrange
            var userId = "user123";
            var goalDto = new GoalDTO
            {
                GoalType = "Manual", // Meta manual
                AutoGoalType = null,
                Calories = 500, // Calorias fora da faixa permitida (menor que 1200)
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _goalService.CreateGoalAsync(userId, goalDto));
        }

        [Fact]
        public async Task UpdateGoalAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
        {
            // Arrange
            var goalDto = new GoalDTO
            {
                GoalType = "Manual",
                AutoGoalType = null,
                Calories = 2500,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
            };

            // Act
            var success = await _goalService.UpdateGoalAsync("user123", goalDto); // ID inválido

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task DeleteGoalAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
        {
            var userId = "user123";
            // Act
            var success = await _goalService.DeleteGoalAsync(userId); // ID inválido

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsAutomaticaButAutoGoalTypeIsNull()
        {
            // Arrange
            var user = new User { Id = "user123", Weight = 70, Height = 175, Gender = Gender.Male, ActivityLevel = ActivityLevel.ModeratelyActive, Birthdate = new DateOnly(1995, 5, 10) };
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Automatica,
                AutoGoalType = null, // AutoGoalType não pode ser nulo
                Calories = 2500,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var (success, errorMessage) = _goalService.ValidateAndCalculateGoal(user, goal);

            // Assert
            Assert.False(success);
            Assert.Equal("O tipo de meta automática deve ter um valor definido para AutoGoalType.", errorMessage);
        }

        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsManualButCaloriesAreNull()
        {
            // Arrange
            var user = new User { Id = "user123", Weight = 70, Height = 175, Gender = Gender.Male, ActivityLevel = ActivityLevel.ModeratelyActive, Birthdate = new DateOnly(1995, 5, 10) };
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Manual,
                AutoGoalType = null,
                Calories = null, // Calorias não definidas
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var (success, errorMessage) = _goalService.ValidateAndCalculateGoal(user, goal);

            // Assert
            Assert.False(success);
            Assert.Equal("O valor das calorias deve ser definido para metas manuais.", errorMessage);
        }

        [Theory]
        [InlineData(AutoGoalType.PerderPeso, -500)]
        [InlineData(AutoGoalType.ManterPeso, 0)]
        [InlineData(AutoGoalType.GanharPeso, 500)]
        public void ValidateGoal_ShouldCalculateCorrectCalories_WhenGoalIsAutomatic(AutoGoalType autoGoalType, int expectedDifference)
        {
            // Arrange
            var user = new User { Id = "user123", Weight = 70, Height = 175, Gender = Gender.Male, ActivityLevel = ActivityLevel.ModeratelyActive, Birthdate = new DateOnly(1995, 5, 10) };
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Automatica,
                AutoGoalType = autoGoalType,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Calcula o BMR e calorias esperadas
            double bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age() + 5; // Para "Male"
            double activityMultiplier = 1.55; // Moderadamente Ativo
            int expectedCalories = (int)(bmr * activityMultiplier) + expectedDifference;

            // Act
            var (success, errorMessage) = _goalService.ValidateAndCalculateGoal(user, goal);

            // Assert
            Assert.True(success);
            Assert.Null(errorMessage);
            Assert.Equal(expectedCalories, goal.Calories);
        }

        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenStartDateIsAfterEndDate()
        {
            // Arrange
            var user = new User { Id = "user123", Weight = 70, Height = 175, Gender = Gender.Male, ActivityLevel = ActivityLevel.ModeratelyActive, Birthdate = new DateOnly(1995, 5, 10) };
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Manual,
                Calories = 2000,
                StartDate = DateTime.UtcNow.AddMonths(1),
                EndDate = DateTime.UtcNow
            };

            // Act
            var (success, errorMessage) = _goalService.ValidateAndCalculateGoal(user, goal);

            // Assert
            Assert.False(success);
            Assert.Equal("A data de início deve ser anterior à data de término.", errorMessage);
        }
    }
}
