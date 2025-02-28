using Microsoft.AspNetCore.Identity;
using backend.Models;
using backend.Models.Ingredients;

namespace backend.Data
{
    //Mário
    public class UserSeeder
    {
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // estudante
            if (await userManager.FindByEmailAsync("user@example.com") == null)
            {
                var user = new User
                {
                    UserName = "user@example.com",
                    Email = "user@example.com",
                    Name = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "UserPass123!");
                await userManager.AddToRoleAsync(user, "User");
            }
            if (await userManager.FindByEmailAsync("mariodelgadinho28@gmail.com") == null)
            {
                var user = new User
                {
                    UserName = "mariodelgadinho28",
                    Email = "mariodelgadinho28@gmail.com",
                    Name = "mario",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Mario2811.");
                await userManager.AddToRoleAsync(user, "User");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // administrador
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "Admin",
                    EmailConfirmed = true,
                    UserIngredients = [new UserIngredient { IngredientName = "Potato", Quantity = 1 }, new UserIngredient { IngredientName = "Apple", Quantity = 3.5f, Unit = "Kg"  }]
                };
                await userManager.CreateAsync(admin, "AdminPass123!");
                await userManager.AddToRoleAsync(admin, "User");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
