using backend.Data;
using backend.Models;

namespace backend.Services
{
    //Mário
    public class UserLogService
    {
        private readonly ICareServerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserLogService(ICareServerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string? userId, string message)
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var log = new UserLog
            {
                UserId = userId,
                Message = message,
                TimeStamp = DateTime.UtcNow,
                IpAddress = ipAddress,
            };

            _context.UserLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}