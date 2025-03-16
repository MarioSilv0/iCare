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
    /// <summary>
    /// The <c>GoalServiceTests</c> class contains unit tests for the <see cref="GoalService"/> class.
    /// These tests verify the business logic for creating, updating, deleting, and retrieving goals, 
    /// as well as validating goal inputs and handling exceptions.
    /// </summary>
    public class GoalServiceTests : IClassFixture<ICareContextFixture>, IDisposable
    {
        private readonly ICareServerContext _context;
        private readonly GoalService _goalService;

        /// <summary>
        /// Initializes a new instance of the <c>GoalServiceTests</c> class.
        /// This sets up the context and the service to test, along with cleaning the database between tests.
        /// </summary>
        public GoalServiceTests(ICareContextFixture fixture)
        {
            _context = fixture.DbContext;
            _goalService = new GoalService(_context);
        }

        /// <summary>
        /// Cleans up the database by removing goals and users after each test.
        /// </summary>
        public void Dispose()
        {
            _context.Goals.RemoveRange(_context.Goals);
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }

        /// <summary>
        /// Tests the <see cref="GoalService.GetLatestGoalByUserIdAsync"/> method.
        /// Verifies that the latest goal for the user is returned when the user has goals.
        /// </summary>
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

        /// <summary>
        /// Tests the <see cref="GoalService.GetLatestGoalByUserIdAsync"/> method.
        /// Verifies that the method returns <c>null</c> when the user has no goals.
        /// </summary>
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

        /// <summary>
        /// Tests the <see cref="GoalService.CreateGoalAsync"/> method.
        /// Verifies that the correct number of calories is calculated for an automatic goal.
        /// </summary>
        [Fact]
        public async Task CreateGoalAsync_ShouldCalculateCaloriesForAutomaticGoal_WhenGoalTypeIsAutomatica()
        {
            // Arrange
            var userId = "user123";
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
            Assert.Equal(1837, createdGoal.Calories); // Assert the calculated calories
        }

        /// <summary>
        /// Tests the <see cref="GoalService.CreateGoalAsync"/> method.
        /// Verifies that an exception is thrown when the calorie value for a manual goal is out of the allowed range.
        /// </summary>
        [Fact]
        public async Task CreateGoalAsync_ShouldThrowInvalidOperationException_WhenCaloriesAreOutOfRange()
        {
            // Arrange
            var userId = "user123";
            var goalDto = new GoalDTO
            {
                GoalType = "Manual",
                AutoGoalType = null,
                Calories = 500, // Invalid calories (less than 1200)
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _goalService.CreateGoalAsync(userId, goalDto));
        }

        /// <summary>
        /// Tests the <see cref="GoalService.UpdateGoalAsync"/> method.
        /// Verifies that the method returns <c>false</c> when trying to update a non-existent goal.
        /// </summary>
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
            var success = await _goalService.UpdateGoalAsync("user123", 999, goalDto); // Invalid goal ID

            // Assert
            Assert.False(success);
        }

        /// <summary>
        /// Tests the <see cref="GoalService.DeleteGoalAsync"/> method.
        /// Verifies that the method returns <c>false</c> when trying to delete a non-existent goal.
        /// </summary>
        [Fact]
        public async Task DeleteGoalAsync_ShouldReturnFalse_WhenGoalDoesNotExist()
        {
            // Act
            var success = await _goalService.DeleteGoalAsync(999); // Invalid goal ID

            // Assert
            Assert.False(success);
        }

        /// <summary>
        /// Tests the <see cref="GoalService.ValidateGoal"/> method.
        /// Verifies that the validation fails when the goal type is "Automatica" but no AutoGoalType is provided.
        /// </summary>
        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsAutomaticaButAutoGoalTypeIsNull()
        {
            // Arrange
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Automatica,
                AutoGoalType = null, // AutoGoalType cannot be null
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

        /// <summary>
        /// Tests the <see cref="GoalService.ValidateGoal"/> method.
        /// Verifies that the validation fails when the goal type is "Manual" but no calories are provided.
        /// </summary>
        [Fact]
        public void ValidateGoal_ShouldReturnFalse_WhenGoalTypeIsManualButCaloriesAreNull()
        {
            // Arrange
            var goal = new Goal
            {
                UserId = "user123",
                GoalType = GoalType.Manual,
                AutoGoalType = null,
                Calories = null, // Calories are required for manual goals
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
