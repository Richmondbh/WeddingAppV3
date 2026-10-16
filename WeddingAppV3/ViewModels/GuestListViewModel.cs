using WeddingAppV3.Models;

namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// Everything the guest list page needs: the guests after filtering and
    /// sorting, the current filter choices, and a few totals.
    /// </summary>
    public class GuestListViewModel
    {
        public List<Guest> Guests { get; set; } = new List<Guest>();

        // Current choices, so the form can show what is selected.
        public string Filter { get; set; }

        public string Sort { get; set; }

        public string Search { get; set; }

        // Totals for all guests, not only the filtered ones.
        public int TotalGuests { get; set; }

        public int AttendingCount { get; set; }

        public int NotAttendingCount { get; set; }
    }
}
