using Microsoft.AspNetCore.Identity;
using backend.Models;

namespace backend.Data
{
    // Nível 3
    public class UserLogSeeder
    {
        public static async Task SeedReservationsAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            // Obter o estudante
            var user = await userManager.FindByEmailAsync("user@example.com");
            if (user == null) return;
                        
            context.UserLog.Add(new UserLog
            {
                UserId = user.Id,
                Message = user.Id + user.Name + "logou",
                TimeStamp = DateTime.Now,
            });
          
            await context.SaveChangesAsync();
        }
    }
}
