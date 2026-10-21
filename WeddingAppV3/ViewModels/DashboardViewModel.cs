namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// Everything the admin dashboard shows. It is built from smaller ViewModels:
    /// the calculated statistics, the attendance groups and the latest messages.
    /// </summary>
    public class DashboardViewModel
    {
        public StatisticsViewModel Statistics { get; set; } = new StatisticsViewModel();

        public List<AttendanceGroupViewModel> AttendanceGroups { get; set; } = new List<AttendanceGroupViewModel>();

        public int WeddingInfoCount { get; set; }

        public int UserCount { get; set; }

        // Latest guests who left a message.
        public List<GuestMessageViewModel> RecentMessages { get; set; } = new List<GuestMessageViewModel>();
    }
}
