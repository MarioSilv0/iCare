using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    /// <summary>
    /// The <c>RoleSeeder</c> class contains a method for seeding default roles in the system.
    /// It ensures that roles like "User" and "Admin" exist in the database by checking if they already exist,
    /// and creating them if they do not.
    /// </summary>
    public static class RoleSeeder
    {
        /// <summary>
        /// Seeds the roles into the role manager if they do not already exist.
        /// This method ensures that the "User" and "Admin" roles are created in the system.
        /// </summary>
        /// <param name="roleManager">The role manager used to manage roles.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Role names to be seeded
            string[] roleNames = { "User", "Admin" };

            // Loop through each role name and check if it exists
            foreach (var roleName in roleNames)
            {
                // If the role does not exist, create it
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
