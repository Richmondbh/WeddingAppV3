namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// One user account on the "Users and roles" admin page.
    /// Only safe information is included (no password hash, no security stamp).
    /// </summary>
    public class UserAccountViewModel
    {
        public string Id { get; set; } = "";

        public string Email { get; set; } = "";

        public bool IsAdmin { get; set; }

        public bool IsUser { get; set; }

        // True when the user has the claim CanManageGuests = true.
        public bool CanManageGuests { get; set; }

        // True for the admin who is looking at the page.
        public bool IsCurrentUser { get; set; }
    }
}
