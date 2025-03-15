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
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow.AddMonths(1)
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
            var userId = "user123"; // Mock do ID do usuário
            var goalDto = new GoalDTO
            {
                GoalType = "Manual", // Meta manual
                AutoGoalType = null,
                Calories = 500, // Calorias fora da faixa permitida (menor que 1200)
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
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
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act
            var success = await _goalService.UpdateGoalAsync("user123", 999, goalDto); // ID inválido

            // Assert
            Assert.False(success);
        }

        [Fact]
        public async Task DeleteGoalAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
        {
            // Act
            var success = await _goalService.DeleteGoalAsync(999); // ID inválido

            // Assert
            Assert.False(success);
        }

        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsAutomaticaButAutoGoalTypeIsNull()
        {
            // Arrange
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
            var (success, errorMessage) = _goalService.ValidateGoal(goal);

            // Assert
            Assert.False(success);
            Assert.Equal("O tipo de meta automática deve ter um valor definido para AutoGoalType.", errorMessage);
        }

        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsManualButCaloriesAreNull()
        {
            // Arrange
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
            var (success, errorMessage) = _goalService.ValidateGoal(goal);

            // Assert
            Assert.False(success);
            Assert.Equal("O valor das calorias deve ser definido para metas manuais.", errorMessage);
        }
    }
}
