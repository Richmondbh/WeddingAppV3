using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Models;

namespace WeddingAppV3.Data
{
    /// <summary>
    /// Runs once every time the application starts (called from Program.cs).
    /// 1. Applies any migrations that have not been applied yet.
    /// 2. Creates the roles "Admin" and "User" if they do not exist.
    /// 3. Creates three test accounts if they do not exist:
    ///      admin@wedding.se   / Admin123!    role Admin + claim CanManageGuests
    ///      user@wedding.se    / User123!     role User (no extra rights)
    ///      planner@wedding.se / Planner123!  role User + claim CanManageGuests
    ///    The planner shows claims-based authorization: not an admin, but the
    ///    claim still gives access to the guest list.
    /// Running it again does nothing, because everything is checked first.
    /// </summary>
    public static class SeedData
    {
        public const string AdminRole = "Admin";
        public const string UserRole = "User";
        public const string ManageGuestsClaim = "CanManageGuests";

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
            ApplicationUser admin = await CreateUserAsync(userManager, "admin@wedding.se", "Admin123!", AdminRole);
            await AddClaimIfMissingAsync(userManager, admin, ManageGuestsClaim, "true");

            await CreateUserAsync(userManager, "user@wedding.se", "User123!", UserRole);

            ApplicationUser planner = await CreateUserAsync(userManager, "planner@wedding.se", "Planner123!", UserRole);
            await AddClaimIfMissingAsync(userManager, planner, ManageGuestsClaim, "true");
        }

        // Gives the user a claim (stored in the AspNetUserClaims table) if they do not have it.
        private static async Task AddClaimIfMissingAsync(UserManager<ApplicationUser> userManager,
            ApplicationUser user, string claimType, string claimValue)
        {
            IList<Claim> claims = await userManager.GetClaimsAsync(user);

            if (!claims.Any(c => c.Type == claimType && c.Value == claimValue))
            {
                await userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            }
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
