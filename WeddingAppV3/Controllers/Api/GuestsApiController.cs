using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers.Api
{
    /// <summary>
    /// Web API for guests. Returns JSON instead of HTML pages.
    /// It inherits from ControllerBase (not Controller) because an API has no views.
    ///
    /// Endpoints:
    ///   GET /api/guests                       all guests (can be filtered and sorted)
    ///   GET /api/guests?status=attending      only attending guests
    ///   GET /api/guests?sort=name_desc        sorted Z-A
    ///   GET /api/guests/5                     one guest with id 5
    ///   GET /api/guests/attending             names of attending guests (anonymous type)
    /// </summary>
    [Route("api/guests")]
    [ApiController]
    public class GuestsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GuestsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/guests?status=attending&sort=name
        // status: attending, declined or noresponse (optional)
        // sort:   name (default), name_desc or newest (optional)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestViewModel>>> GetGuests(string status = null, string sort = null)
        {
            IQueryable<Guest> query = _context.Guests;

            // Filtering with Where()
            if (status == "attending")
            {
                query = query.Where(g => g.WillAttend == true);
            }
            else if (status == "declined")
            {
                query = query.Where(g => g.WillAttend == false);
            }
            else if (status == "noresponse")
            {
                query = query.Where(g => g.WillAttend == null);
            }
            else if (!string.IsNullOrEmpty(status))
            {
                // 400 Bad Request with a message that explains what went wrong.
                return BadRequest(new { message = "Unknown status. Use attending, declined or noresponse." });
            }

            // Sorting with OrderBy() and OrderByDescending()
            if (string.IsNullOrEmpty(sort) || sort == "name")
            {
                query = query.OrderBy(g => g.Name);
            }
            else if (sort == "name_desc")
            {
                query = query.OrderByDescending(g => g.Name);
            }
            else if (sort == "newest")
            {
                query = query.OrderByDescending(g => g.RegisteredOn);
            }
            else
            {
                return BadRequest(new { message = "Unknown sort order. Use name, name_desc or newest." });
            }

            // Projection with Select() into a ViewModel, read asynchronously.
            List<GuestViewModel> guests = await query
                .Select(g => new GuestViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    Email = g.Email,
                    WillAttend = g.WillAttend,
                    NumberOfAttendants = g.NumberOfAttendants
                })
                .ToListAsync();

            // 200 OK with the list as JSON.
            return Ok(guests);
        }

        // GET: /api/guests/5
        // ":int" means the route only matches when id is a number.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GuestDetailsViewModel>> GetGuest(int id)
        {
            GuestDetailsViewModel guest = await _context.Guests
                .Where(g => g.Id == id)
                .Select(g => new GuestDetailsViewModel
                {
                    Id = g.Id,
                    Name = g.Name,
                    Email = g.Email,
                    PhoneNumber = g.PhoneNumber,
                    WillAttend = g.WillAttend,
                    NumberOfAttendants = g.NumberOfAttendants,
                    Message = g.Message,
                    RegisteredOn = g.RegisteredOn
                })
                .FirstOrDefaultAsync();

            if (guest == null)
            {
                // 404 Not Found with a message.
                return NotFound(new { message = $"No guest with id {id} was found." });
            }

            return Ok(guest);
        }

        // GET: /api/guests/attending
        // Uses an ANONYMOUS TYPE: new { ... } without a class name.
        // Only the name and the number of attendants are sent to the client.
        [HttpGet("attending")]
        public async Task<IActionResult> GetAttendingGuests()
        {
            var attendingGuests = await _context.Guests
                .Where(g => g.WillAttend == true)
                .OrderBy(g => g.Name)
                .Select(g => new
                {
                    g.Name,
                    Attendants = g.NumberOfAttendants
                })
                .ToListAsync();

            return Ok(new
            {
                count = attendingGuests.Count,
                guests = attendingGuests
            });
        }
    }
}
