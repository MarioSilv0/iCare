using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace backendtest
{
    /// <summary>
    /// The <c>UserLogServiceTests</c> class contains unit tests for the <see cref="UserLogService"/> class.
    /// These tests ensure that user logs are correctly added to the database when calling the <see cref="UserLogService.LogAsync"/> method.
    /// </summary>
    /// <author>Mário Silva - 202000500</author>
    public class UserLogServiceTests
    {
        private readonly UserLogService _userLogService;
        private readonly ICareServerContext _context;

        /// <summary>
        /// Initializes a new instance of the <c>UserLogServiceTests</c> class.
        /// Sets up the in-memory database and mocks the <see cref="IHttpContextAccessor"/> for simulating HTTP context in the service.
        /// </summary>
        public UserLogServiceTests()
        {
            var options = new DbContextOptionsBuilder<ICareServerContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new ICareServerContext(options);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext());

            _userLogService = new UserLogService(_context, mockHttpContextAccessor.Object);
        }

        /// <summary>
        /// Tests the <see cref="UserLogService.LogAsync"/> method to ensure that a log is successfully added to the database.
        /// </summary>
        /// <remarks>
        /// This test verifies that when the <see cref="LogAsync"/> method is called with a user ID and a log message,
        /// the log is correctly saved to the database and can be retrieved.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Fact]
        public async Task LogAsync_ShouldAddLogToDatabase()
        {
            // Arrange
            var userId = "12345";
            var message = "Test log message";

            // Act
            await _userLogService.LogAsync(userId, message);

            // Assert
            var logs = _context.UserLogs.ToList();
            Assert.Single(logs);
            Assert.Equal(userId, logs[0].UserId);
            Assert.Equal(message, logs[0].Message);
        }
    }
}
