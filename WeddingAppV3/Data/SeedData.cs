using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Models;

namespace WeddingAppV3.Data
{
    /// <summary>
    /// Runs once every time the application starts (called from Program.cs).
    /// 1. Applies any migrations that have not been applied yet.
    /// 2. Creates the roles "Admin" and "User" if they do not exist.
    /// 3. Creates two test accounts 
    /// </summary>
    public static class SeedData
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public static async Task InitializeAsync(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Same as running Update-Database: creates/updates the tables if needed.
            await context.Database.MigrateAsync();

            // The roles.
            string[] roles = { AdminRole, UserRole };

            foreach (string roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // The test accounts.
            await CreateUserAsync(userManager, "admin@rich.se", "Admin123!", AdminRole);
            await CreateUserAsync(userManager, "user@rich.se", "User123!", UserRole);
        }

        // Creates the account if it is missing and makes sure it has the role.
        private static async Task<ApplicationUser> CreateUserAsync(UserManager<ApplicationUser> userManager,
            string email, string password, string role)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create {email}: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}
