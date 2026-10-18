namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// Everything the guest list page needs: the guests after filtering and
    /// sorting, the current filter choices, and a few totals.
    /// the list now holds GuestViewModel objects instead of Guest entities.
    /// </summary>
    public class GuestListViewModel
    {
        public List<GuestViewModel> Guests { get; set; } = new List<GuestViewModel>();

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
