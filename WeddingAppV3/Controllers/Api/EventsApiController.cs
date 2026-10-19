using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers.Api
{
    /// <summary>
    /// Web API for the events of the wedding day. Public (read only).
    ///
    /// Endpoints:
    ///   GET /api/events                    all events
    ///   GET /api/events?location=hall      events whose location contains "hall"
    ///   GET /api/events/3                  one event with id 3
    ///   GET /api/events/overview           short overview (anonymous type)
    /// </summary>
    [Route("api/events")]
    [ApiController]
    public class EventsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/events  or  /api/events?location=hall
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventViewModel>>> GetEvents(string location = null)
        {
            IQueryable<Event> query = _context.Events;

            if (!string.IsNullOrWhiteSpace(location))
            {
                string term = location.Trim().ToLower();
                query = query.Where(e => e.Location.ToLower().Contains(term));
            }

            List<EventViewModel> events = await query
                .OrderBy(e => e.Id)
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Date = e.Date,
                    Time = e.Time,
                    Location = e.Location,
                    Description = e.Description
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET: /api/events/3
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EventViewModel>> GetEvent(int id)
        {
            EventViewModel weddingEvent = await _context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Title = e.Title,
                    Date = e.Date,
                    Time = e.Time,
                    Location = e.Location,
                    Description = e.Description
                })
                .FirstOrDefaultAsync();

            if (weddingEvent == null)
            {
                return NotFound(new { message = $"No event with id {id} was found." });
            }

            return Ok(weddingEvent);
        }

        // GET: /api/events/overview
        // Anonymous type: only the title and one combined "startsAt" text.
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var events = await _context.Events
                .OrderBy(e => e.Id)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    StartsAt = e.Date + " at " + e.Time
                })
                .ToListAsync();

            return Ok(new
            {
                numberOfEvents = events.Count,
                events
            });
        }
    }
}
