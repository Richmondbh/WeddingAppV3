using System.ComponentModel.DataAnnotations;

namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// Everything shown on the details page of one guest
    /// (and returned by GET /api/guests/{id}).
    /// </summary>
    public class GuestDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        [Display(Name = "Email address")]
        public string Email { get; set; } = "";

        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Will attend")]
        public bool? WillAttend { get; set; }

        [Display(Name = "Number of attendants")]
        public int NumberOfAttendants { get; set; }

        [Display(Name = "Message to the hosts")]
        public string Message { get; set; }

        [Display(Name = "Registered")]
        public DateTime RegisteredOn { get; set; }

        public string Status => WillAttend == true ? "Attending"
            : WillAttend == false ? "Not attending"
            : "No response";
    }
}
