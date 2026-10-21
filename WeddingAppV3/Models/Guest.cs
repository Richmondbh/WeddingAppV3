using System.ComponentModel.DataAnnotations;

namespace WeddingAppV3.Models
{
    /// <summary>
    /// A guest invited to the wedding. A guest is first registered with name and
    /// email, and later answers the RSVP, which fills in the remaining fields.
    /// Stored in the Guests table.
    /// </summary>
    public class Guest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kindly enter your name.")]
        [StringLength(60, MinimumLength = 2,
            ErrorMessage = "The name must be between 2 and 60 characters.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        // Added in the second migration (AddPhoneNumber). 
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        // Null means the guest has not answered the RSVP yet.
        [Display(Name = "Will attend")]
        public bool? WillAttend { get; set; }

        [Range(0, 10, ErrorMessage = "Number of attendants must be between 0 and 10.")]
        [Display(Name = "Number of attendants")]
        public int NumberOfAttendants { get; set; }

        [StringLength(250, ErrorMessage = "The message may not exceed 250 characters.")]
        [Display(Name = "Message to the hosts")]
        public string Message { get; set; }

        [Display(Name = "Registered")]
        public DateTime RegisteredOn { get; set; }
    }
}
