namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// One group of guests with the same RSVP answer (Attending, Declined or
    /// No response). Produced with GroupBy() in StatisticsService.
    /// </summary>
    public class AttendanceGroupViewModel
    {
        public string Status { get; set; } = "";

        public int NumberOfGuests { get; set; }

        public int NumberOfAttendants { get; set; }

        // Share of all guests, e.g. 33.3
        public double Percentage { get; set; }
    }
}
