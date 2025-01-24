using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace backendtest
{
    public class UserLogServiceTests
    {
        private readonly UserLogService _userLogService;
        private readonly ICareServerContext _context;

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