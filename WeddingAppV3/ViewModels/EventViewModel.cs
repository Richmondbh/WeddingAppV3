namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// One part of the wedding day as shown to visitors
    /// (public Event page and GET /api/events).
    /// </summary>
    public class EventViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public string Date { get; set; } = "";

        public string Time { get; set; } = "";

        public string Location { get; set; } = "";

        public string Description { get; set; }
    }
}
