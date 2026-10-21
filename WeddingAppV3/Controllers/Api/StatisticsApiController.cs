using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Services;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers.Api
{
    /// <summary>
    /// Web API that returns CALCULATED data instead of plain database rows.
    /// Only for admins.
    ///
    /// Endpoints:
    ///   GET /api/statistics                                  totals and percentages
    ///   GET /api/statistics/attendance                       guests grouped by RSVP answer
    ///   GET /api/statistics/registrations?page=1&pageSize=5  newest registrations, page by page
    ///   GET /api/statistics/events                           events grouped by date
    /// </summary>
    [Route("api/statistics")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StatisticsApiController : ControllerBase
    {
        private readonly StatisticsService _statistics;
        private readonly ApplicationDbContext _context;

        public StatisticsApiController(StatisticsService statistics, ApplicationDbContext context)
        {
            _statistics = statistics;
            _context = context;
        }

        // GET: /api/statistics
        [HttpGet]
        public async Task<ActionResult<StatisticsViewModel>> GetSummary()
        {
            StatisticsViewModel summary = await _statistics.GetSummaryAsync();
            return Ok(summary);
        }

        // GET: /api/statistics/attendance
        [HttpGet("attendance")]
        public async Task<ActionResult<IEnumerable<AttendanceGroupViewModel>>> GetAttendance()
        {
            List<AttendanceGroupViewModel> groups = await _statistics.GetAttendanceGroupsAsync();
            return Ok(groups);
        }

        // GET: /api/statistics/registrations?page=1&pageSize=5
        // Paging with Skip() and Take(): page 1 = the 5 newest, page 2 = the next 5 ...
        [HttpGet("registrations")]
        public async Task<IActionResult> GetRegistrations(int page = 1, int pageSize = 5)
        {
            if (page < 1 || pageSize < 1 || pageSize > 50)
            {
                return BadRequest(new { message = "page must be 1 or more and pageSize between 1 and 50." });
            }

            int totalGuests = await _context.Guests.CountAsync();

            var guests = await _context.Guests
                .OrderByDescending(g => g.RegisteredOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new
                {
                    g.Name,
                    g.RegisteredOn,
                    Status = g.WillAttend == true ? "Attending"
                        : g.WillAttend == false ? "Declined"
                        : "No response"
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalGuests,
                totalPages = (int)Math.Ceiling(totalGuests / (double)pageSize),
                guests
            });
        }

        // GET: /api/statistics/events
        // Groups the events by date and lists the titles in time order.
        [HttpGet("events")]
        public async Task<IActionResult> GetEventSummary()
        {
            // Read only the needed columns from the database ...
            var events = await _context.Events
                .Select(e => new { e.Date, e.Time, e.Title })
                .ToListAsync();

            // ... then group them in memory with LINQ to Objects.
            var days = events
                .GroupBy(e => e.Date)
                .Select(day => new
                {
                    Date = day.Key,
                    NumberOfEvents = day.Count(),
                    FirstStart = day.Min(e => e.Time),
                    Events = day.OrderBy(e => e.Time).Select(e => e.Time + " " + e.Title).ToList()
                })
                .ToList();

            return Ok(days);
        }
    }
}
