using System.ComponentModel.DataAnnotations;

namespace WeddingAppV3.Models
{
    /// <summary>
    /// A block of text shown on the home page, such as the welcome message, venue
    /// information or contact details. Managed from the admin section.
    /// Stored in the WeddingInfos table.
    /// </summary>
    public class WeddingInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a heading.")]
        [StringLength(80)]
        public string Heading { get; set; }

        [Required(ErrorMessage = "Please enter some content.")]
        [StringLength(1000)]
        public string Content { get; set; }

        // Lower numbers are shown first on the home page.
        [Range(0, 100)]
        [Display(Name = "Display order")]
        public int DisplayOrder { get; set; }
    }
}
