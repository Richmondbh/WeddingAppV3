using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Services;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// The administration dashboard. The statistics come from StatisticsService
    /// (the same service the statistics Web API uses).
    /// </summary>
    [Authorize(Roles = "Admin")] // only users with the Admin role
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly StatisticsService _statistics;

        public AdminController(ApplicationDbContext context, StatisticsService statistics)
        {
            _context = context;
            _statistics = statistics;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                Statistics = await _statistics.GetSummaryAsync(),
                AttendanceGroups = await _statistics.GetAttendanceGroupsAsync(),
                WeddingInfoCount = await _context.WeddingInfos.CountAsync(),
                UserCount = await _context.Users.CountAsync(),

                // Only the name and the message are projected into a ViewModel.
                RecentMessages = await _context.Guests
                    .Where(g => g.Message != null && g.Message != "")
                    .OrderByDescending(g => g.RegisteredOn)
                    .Take(5)
                    .Select(g => new GuestMessageViewModel
                    {
                        Name = g.Name,
                        Message = g.Message
                    })
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
