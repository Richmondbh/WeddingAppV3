using System.ComponentModel.DataAnnotations;

namespace WeddingAppV3.Models
{
    /// <summary>
    /// One part of the wedding day, for example the ceremony or the dinner.
    /// Date and Time are kept as text because they are only displayed.
    /// this is stored in the Events table.
    /// </summary>
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(80)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter a date.")]
        [StringLength(40)]
        public string Date { get; set; }

        [Required(ErrorMessage = "Please enter a time.")]
        [StringLength(20)]
        public string Time { get; set; }

        [Required(ErrorMessage = "Please enter a location.")]
        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
