namespace WeddingAppV3.ViewModels
{
    /// <summary>
    /// A message a guest wrote to the hosts, shown on the admin dashboard.
    /// Only the name and the message are needed there.
    /// </summary>
    public class GuestMessageViewModel
    {
        public string Name { get; set; } = "";

        public string Message { get; set; } = "";
    }
}
