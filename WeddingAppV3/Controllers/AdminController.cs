using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// The administration dashboard: RSVP statistics calculated from the
    /// database, plus links to guest, event and wedding information management.
    /// </summary>
    [Authorize(Roles = "Admin")] // only users with the Admin role
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = new DashboardViewModel
            {
                TotalGuests = await _context.Guests.CountAsync(),
                AttendingCount = await _context.Guests.CountAsync(g => g.WillAttend == true),
                NotAttendingCount = await _context.Guests.CountAsync(g => g.WillAttend == false),
                NoResponseCount = await _context.Guests.CountAsync(g => g.WillAttend == null),

                // Only guests who will attend are counted in the headcount.
                TotalAttendants = await _context.Guests
                    .Where(g => g.WillAttend == true)
                    .SumAsync(g => g.NumberOfAttendants),

                EventCount = await _context.Events.CountAsync(),
                WeddingInfoCount = await _context.WeddingInfos.CountAsync(),

                // only the name and the message are projected into a ViewModel.
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
