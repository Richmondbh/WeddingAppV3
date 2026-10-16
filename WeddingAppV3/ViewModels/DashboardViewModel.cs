using WeddingAppV3.Models;

namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// RSVP statistics and recent activity for the admin dashboard and all values
    /// are calculated from the database in AdminController.
    /// </summary>
    public class DashboardViewModel
    {
        public int TotalGuests { get; set; }

        public int AttendingCount { get; set; }

        public int NotAttendingCount { get; set; }

        public int NoResponseCount { get; set; }

        // Sum of NumberOfAttendants for the guests who will attend.
        public int TotalAttendants { get; set; }

        public int EventCount { get; set; }

        public int WeddingInfoCount { get; set; }

        // Latest guests who left a message.
        public List<Guest> RecentMessages { get; set; } = new List<Guest>();
    }
}
