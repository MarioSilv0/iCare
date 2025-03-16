using backend.Controllers.Api;
using backend.Data;
using backend.Models;
using backend.Models.Data_Transfer_Objects;
using backend.Models.Enums;
using backend.Models.Goals;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace backendtest.Controllers.Api
{
    /// <summary>
    /// The <c>GoalControllerTests</c> class contains unit tests for the <see cref="GoalController"/> class.
    /// These tests ensure that the methods for creating, retrieving, updating, and deleting goals behave as expected.
    /// </summary>
    public class GoalControllerTests : IClassFixture<ICareContextFixture>
    {
        private readonly GoalController _controller;
        private readonly Mock<IGoalService> _goalServiceMock;

        /// <summary>
        /// Initializes a new instance of the <c>GoalControllerTests</c> class.
        /// This sets up the mock services and controller, and initializes the user claims for authentication.
        /// </summary>
        public GoalControllerTests(ICareContextFixture fixture)
        {
            _goalServiceMock = new Mock<IGoalService>();

            var loggerMock = new Mock<ILogger<GoalController>>();
            _controller = new GoalController(_goalServiceMock.Object, loggerMock.Object);

            var claims = new List<Claim> { new Claim("UserId", "user123") };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        /// <summary>
        /// Test for the <see cref="GoalController.GetCurrentGoalByUserId"/> method.
        /// Ensures that a valid goal for the user is returned with a status of OK.
        /// </summary>
        [Fact]
        public async Task GetLatestGoalByUserId_ShouldReturnOk_WhenGoalExists()
        {
            // Arrange
            var userId = "user123";
            var goal = new Goal { UserId = userId, GoalType = GoalType.Manual, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(4), Calories = 2500 };
            _goalServiceMock.Setup(service => service.GetLatestGoalByUserIdAsync(userId)).ReturnsAsync(goal);

            // Act
            var result = await _controller.GetCurrentGoalByUserId();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnGoal = Assert.IsType<Goal>(okResult.Value);
            Assert.Equal(goal.Id, returnGoal.Id); // Verifies that the returned goal ID matches the expected one
        }

        /// <summary>
        /// Test for the <see cref="GoalController.GetCurrentGoalByUserId"/> method.
        /// Ensures that when no goal exists for the user, a NotFound status is returned.
        /// </summary>
        [Fact]
        public async Task GetLatestGoalByUserId_ShouldReturnNotFound_WhenGoalDoesNotExist()
        {
            // Arrange
            var userId = "user123";
            _goalServiceMock.Setup(service => service.GetLatestGoalByUserIdAsync(userId)).ReturnsAsync((Goal)null);

            // Act
            var result = await _controller.GetCurrentGoalByUserId();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Meta não encontrada.", notFoundResult.Value); // Verifies the correct error message
        }

        /// <summary>
        /// Test for the <see cref="GoalController.GetCurrentGoalByUserId"/> method.
        /// Ensures that an internal server error is returned when an exception occurs during goal retrieval.
        /// </summary>
        [Fact]
        public async Task GetLatestGoalByUserId_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var userId = "user123";
            _goalServiceMock.Setup(service => service.GetLatestGoalByUserIdAsync(userId)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetCurrentGoalByUserId();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro ao acessar a meta.", statusCodeResult.Value);
        }

        /// <summary>
        /// Test for the <see cref="GoalController.CreateGoal"/> method.
        /// Ensures that a goal is created successfully and returns a CreatedAtAction result.
        /// </summary>
        [Fact]
        public async Task CreateGoal_ReturnsCreatedAtAction_WhenGoalIsCreated()
        {
            // Arrange
            var goalDto = new GoalDTO
            {
                GoalType = "Manual",
                Calories = 3000,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            var userId = "user123";

            var goal = new Goal
            {
                UserId = userId,
                GoalType = GoalType.Manual,
                Calories = 3000,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            _goalServiceMock.Setup(service => service.CreateGoalAsync(userId, goalDto)).ReturnsAsync(goal);

            // Act
            var result = await _controller.CreateGoal(goalDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCurrentGoalByUserId", createdResult.ActionName);
            var createdGoal = Assert.IsType<Goal>(createdResult.Value);
            Assert.Equal(userId, createdGoal.UserId);
        }

        /// <summary>
        /// Test for the <see cref="GoalController.UpdateGoal"/> method.
        /// Ensures that a goal update returns a NoContent result when the goal is updated successfully.
        /// </summary>
        [Fact]
        public async Task UpdateGoal_ReturnsNoContent_WhenGoalIsUpdated()
        {
            // Arrange
            var userId = "user123";
            var goal = new Goal
            {
                Id = 1,
                UserId = userId,
                GoalType = GoalType.Manual,
                Calories = 3000,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };

            var goalDto = new GoalDTO
            {
                GoalType = "Manual",
                Calories = 3000,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(8)
            };
            _goalServiceMock.Setup(service => service.UpdateGoalAsync(userId, 1, goalDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateGoal(goal.Id, goalDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Test for the <see cref="GoalController.DeleteGoal"/> method.
        /// Ensures that a goal is deleted successfully and returns a NoContent result.
        /// </summary>
        [Fact]
        public async Task DeleteGoal_ReturnsNoContent_WhenGoalIsDeleted()
        {
            // Arrange
            var goal = new Goal
            {
                Id = 1,
                UserId = "user123",
                GoalType = GoalType.Manual,
                Calories = 3000,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7)
            };
            _goalServiceMock.Setup(service => service.DeleteGoalAsync(goal.Id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteGoal(goal.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Test for the <see cref="GoalController.DeleteGoal"/> method.
        /// Ensures that when the goal does not exist, a NotFound result is returned.
        /// </summary>
        [Fact]
        public async Task DeleteGoal_ReturnsNotFound_WhenGoalDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteGoal(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
