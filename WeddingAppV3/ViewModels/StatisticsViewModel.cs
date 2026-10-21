namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// Calculated RSVP statistics (not stored in the database).
    /// Used by the admin dashboard and returned by GET /api/statistics.
    /// </summary>
    public class StatisticsViewModel
    {
        public int TotalGuests { get; set; }

        public int AttendingGuests { get; set; }

        public int DeclinedGuests { get; set; }

        public int NoResponseGuests { get; set; }

        // Sum of NumberOfAttendants for attending guests (the real headcount).
        public int TotalAttendants { get; set; }

        // Attending guests as a percentage of all guests, e.g. 70.8
        public double AttendancePercentage { get; set; }

        // Guests who have answered (yes or no) as a percentage of all guests.
        public double ResponseRate { get; set; }

        // Average party size for the attending guests.
        public double AverageAttendantsPerGuest { get; set; }

        public int NumberOfEvents { get; set; }
    }
}
