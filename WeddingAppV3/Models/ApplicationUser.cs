using Microsoft.AspNetCore.Identity;

namespace WeddingAppV3.Models
{
    /// <summary>
    /// A user account in the application (someone who can log in).
    /// IdentityUser already contains Email, UserName, PasswordHash and more.
    /// This class lets us add our own user properties later if we need them.
    /// Note: a user account is NOT the same thing as a wedding Guest.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
    }
}
