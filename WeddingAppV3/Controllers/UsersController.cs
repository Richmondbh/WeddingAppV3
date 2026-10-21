using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Models;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// Role and claim management (Admin only). Lists all user accounts and lets
    /// the admin give or remove the roles Admin and User and the claim
    /// CanManageGuests. New accounts that register themselves start without a role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private const string ManageGuestsClaim = "CanManageGuests";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            string currentUserId = _userManager.GetUserId(User);
            List<UserAccountViewModel> model = new List<UserAccountViewModel>();

            foreach (ApplicationUser user in users)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                IList<Claim> claims = await _userManager.GetClaimsAsync(user);

                model.Add(new UserAccountViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    IsAdmin = roles.Contains("Admin"),
                    IsUser = roles.Contains("User"),
                    CanManageGuests = claims.Any(c => c.Type == ManageGuestsClaim && c.Value == "true"),
                    IsCurrentUser = user.Id == currentUserId
                });
            }

            return View(model);
        }

        // POST: /Users/ToggleRole/{id}   (role = "Admin" or "User")
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(string id, string role)
        {
            if (role != "Admin" && role != "User")
            {
                return BadRequest();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                // Protects the admin from locking themself out.
                if (role == "Admin" && user.Id == _userManager.GetUserId(User))
                {
                    TempData["StatusMessage"] = "You cannot remove your own Admin role.";
                    return RedirectToAction(nameof(Index));
                }

                await _userManager.RemoveFromRoleAsync(user, role);
                TempData["StatusMessage"] = $"{user.Email} no longer has the role {role}.";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, role);
                TempData["StatusMessage"] = $"{user.Email} now has the role {role}.";
            }

            await RefreshIfCurrentUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Users/ToggleGuestManager/{id}
        // Adds or removes the claim CanManageGuests = true.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleGuestManager(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            Claim existing = claims.FirstOrDefault(c => c.Type == ManageGuestsClaim);

            if (existing != null)
            {
                await _userManager.RemoveClaimAsync(user, existing);
                TempData["StatusMessage"] = $"{user.Email} can no longer manage guests.";
            }
            else
            {
                await _userManager.AddClaimAsync(user, new Claim(ManageGuestsClaim, "true"));
                TempData["StatusMessage"] = $"{user.Email} can now manage guests.";
            }

            await RefreshIfCurrentUserAsync(user);
            return RedirectToAction(nameof(Index));
        }

        // Roles and claims are stored in the login cookie. If the admin changed their
        // own account, the cookie is renewed so the change is visible at once.
        // Other users see the change the next time they log in.
        private async Task RefreshIfCurrentUserAsync(ApplicationUser user)
        {
            if (user.Id == _userManager.GetUserId(User))
            {
                await _signInManager.RefreshSignInAsync(user);
            }
        }
    }
}
