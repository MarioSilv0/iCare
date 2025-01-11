using Microsoft.AspNetCore.Identity;
using backend.Models;

namespace backend.Data
{
    // Nível 3
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // estudante
            if (await userManager.FindByEmailAsync("User@example.com") == null)
            {
                var user = new User
                {
                    UserName = "user@example.com",
                    Email = "user@example.com",
                    Name = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }

            // administrador
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "Admin",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
