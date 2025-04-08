using backend.Data;
using backend.Models;

namespace backend.Services
{
    /// <summary>
    /// Provides functionality for logging user activities, including storing the user ID, message, timestamp, and IP address.
    /// </summary>
    public class UserLogService
    {
        private readonly ICareServerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLogService"/> class with the specified database context and HTTP context accessor.
        /// </summary>
        /// <param name="context">The database context to interact with.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor used to retrieve the user's IP address.</param>
        public UserLogService(ICareServerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Logs a message related to a user, including the timestamp and the user's IP address.
        /// </summary>
        /// <param name="userId">The ID of the user associated with the log entry.</param>
        /// <param name="message">The message describing the user's activity or action.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogAsync(string? userId, string message)
        {
            // Retrieve the user's IP address from the HTTP context
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            // Create a new log entry
            var log = new UserLog
            {
                UserId = userId,
                Message = message,
                TimeStamp = DateTime.UtcNow,
                IpAddress = ipAddress,
            };

            // Add the log entry to the database and save changes
            _context.UserLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
