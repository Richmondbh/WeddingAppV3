namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// One row in the guest list (MVC page and Web API).
    /// Contains only what the list needs. PhoneNumber, Message and RegisteredOn
    /// from the Guest entity are left out on purpose, so they are not exposed.
    /// </summary>
    public class GuestViewModel
    {
        // Needed for the Details, Edit, RSVP and Delete links.
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        // Null means the guest has not answered yet.
        public bool? WillAttend { get; set; }

        public int NumberOfAttendants { get; set; }

        // Calculated from WillAttend, so it is also included in the JSON from the API.
        public string Status => WillAttend == true ? "Attending"
            : WillAttend == false ? "Not attending"
            : "No response";
    }
}
