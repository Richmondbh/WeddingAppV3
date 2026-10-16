using System.ComponentModel.DataAnnotations;

namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// The data on the RSVP form. Unlike the Guest entity, answering the
    /// attendance question is required here, so the rule lives in this class.
    /// </summary>
    public class RsvpViewModel
    {
        public int Id { get; set; }

        // Shown on the page but not editable, so no validation is needed.
        public string Name { get; set; }

        public string Email { get; set; }

        // Nullable so that [Required] can detect an unanswered question.
        [Required(ErrorMessage = "Please let us know whether you can attend.")]
        [Display(Name = "Will you attend?")]
        public bool? WillAttend { get; set; }

        [Range(0, 10, ErrorMessage = "Number of attendants must be between 0 and 10.")]
        [Display(Name = "Number of attendants (including you)")]
        public int NumberOfAttendants { get; set; }

        [StringLength(250, ErrorMessage = "The message may not exceed 250 characters.")]
        [Display(Name = "Message to the hosts")]
        public string Message { get; set; }
    }
}
