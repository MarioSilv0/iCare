using Microsoft.AspNetCore.Identity;
using backend.Models;
using backend.Models.Ingredients;

namespace backend.Data
{
    /// <summary>
    /// The <c>UserSeeder</c> class is responsible for seeding user data into the system.
    /// It checks if certain users exist in the database and creates them if they do not.
    /// The class ensures that a few predefined users (such as a standard user, an admin user, etc.) are created with appropriate roles.
    /// </summary>
    // Mário
    public class UserSeeder
    {
        /// <summary>
        /// Seeds the users into the database if they do not already exist.
        /// Creates a standard user, an admin user, and assigns roles accordingly.
        /// </summary>
        /// <param name="userManager">The <c>UserManager</c> used to manage user-related operations such as creating users and assigning roles.</param>
        /// <returns>A task that represents the asynchronous operation of seeding users.</returns>
        public static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Checking and creating a standard user if it doesn't exist
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

            // Checking and creating a user with multiple roles if it doesn't exist
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

            // Checking and creating an admin user if it doesn't exist, with user ingredients data
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "Admin",
                    EmailConfirmed = true,
                    // Adding user ingredients for the admin user
                    UserIngredients = new List<UserIngredient>
                    {
                        new UserIngredient { IngredientId = 1, Quantity = 1 },
                        new UserIngredient { IngredientId = 2, Quantity = 3.5f, Unit = "Kg" }
                    }
                };
                await userManager.CreateAsync(admin, "AdminPass123!");
                await userManager.AddToRoleAsync(admin, "User");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
