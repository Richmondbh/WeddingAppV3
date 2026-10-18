using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;
using WeddingAppV3.ViewModels;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// Everything about guests: register, list (with search, filter and sort),
    /// RSVP, details, edit and delete. All database calls are asynchronous.
    /// </summary>
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /filter, sorting and search
        public async Task<IActionResult> Index(string filter, string sort, string search)
        {
            // Starts with all guests and narrow the query step by step.
            // Nothing is sent to the database until ToListAsync is called.
            IQueryable<Guest> query = _context.Guests;

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLower();
                query = query.Where(g => g.Name.ToLower().Contains(term));
            }

            if (filter == "attending")
            {
                query = query.Where(g => g.WillAttend == true);
            }
            else if (filter == "notattending")
            {
                query = query.Where(g => g.WillAttend == false);
            }
            else if (filter == "noresponse")
            {
                query = query.Where(g => g.WillAttend == null);
            }

            if (sort == "name_desc")
            {
                query = query.OrderByDescending(g => g.Name);
            }
            else if (sort == "status")
            {
                // Attending first, then not attending, then no answer.
                query = query.OrderByDescending(g => g.WillAttend).ThenBy(g => g.Name);
            }
            else if (sort == "date")
            {
                query = query.OrderByDescending(g => g.RegisteredOn);
            }
            else
            {
                query = query.OrderBy(g => g.Name);
            }

            // Assignment 3: Select() projects each Guest entity into a GuestViewModel,
            // so only the columns the page needs are read from the database.
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

            GuestListViewModel model = new GuestListViewModel
            {
                Guests = guests,
                Filter = filter,
                Sort = sort,
                Search = search,
                TotalGuests = await _context.Guests.CountAsync(),
                AttendingCount = await _context.Guests.CountAsync(g => g.WillAttend == true),
                NotAttendingCount = await _context.Guests.CountAsync(g => g.WillAttend == false)
            };

            return View(model);
        }

        // GET: /Guest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Assignment 3: the details page gets a GuestDetailsViewModel, not the entity.
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
                return NotFound();
            }

            return View(guest);
        }

        // GET: /Guest/Create  (the "Register Guest" page)
        public IActionResult Create()
        {
            return View(new Guest());
        }

        // POST: /Guest/Create
        // Bind limits which fields the form may set, so RSVP fields cannot be posted here.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,PhoneNumber")] Guest guest)
        {
            if (!ModelState.IsValid)
            {
                // Show the form again with the validation messages.
                return View(guest);
            }

            guest.RegisteredOn = DateTime.Now;

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"{guest.Name} has been registered.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Guest/Rsvp/5
        public async Task<IActionResult> Rsvp(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guest guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            // Copy the stored answers into the form model so an RSVP can be changed later.
            RsvpViewModel model = new RsvpViewModel
            {
                Id = guest.Id,
                Name = guest.Name,
                Email = guest.Email,
                WillAttend = guest.WillAttend,
                NumberOfAttendants = guest.NumberOfAttendants == 0 ? 1 : guest.NumberOfAttendants,
                Message = guest.Message
            };

            return View(model);
        }

        // POST: /Guest/Rsvp/5
        [HttpPost]
        [ValidateAntiForgeryToken]//against cross-site request forgery (CSRF)
        public async Task<IActionResult> Rsvp(int id, RsvpViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // A rule that involves two fields is checked here rather than with an attribute.
            if (model.WillAttend == true && model.NumberOfAttendants < 1)
            {
                ModelState.AddModelError(nameof(model.NumberOfAttendants),
                    "If you are attending, please count at least yourself.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Guest guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            // Updates the existing record instead of creating a new one.
            guest.WillAttend = model.WillAttend;
            guest.NumberOfAttendants = model.WillAttend == true ? model.NumberOfAttendants : 0;
            guest.Message = model.Message;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = guest.Id });
        }

        // GET: /Guest/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            Guest guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // GET: /Guest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guest guest = await _context.Guests.FindAsync(id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // POST: /Guest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Name,Email,PhoneNumber,WillAttend,NumberOfAttendants,Message")] Guest guest)
        {
            if (id != guest.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(guest);
            }

            Guest existing = await _context.Guests.FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            // Copy the edited values onto the tracked entity. RegisteredOn is left unchanged.
            existing.Name = guest.Name;
            existing.Email = guest.Email;
            existing.PhoneNumber = guest.PhoneNumber;
            existing.WillAttend = guest.WillAttend;
            existing.NumberOfAttendants = guest.WillAttend == true ? guest.NumberOfAttendants : 0;
            existing.Message = guest.Message;

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"{existing.Name} has been updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Guest/Delete/5  (asking  for confirmation)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Guest guest = await _context.Guests.FirstOrDefaultAsync(g => g.Id == id);

            if (guest == null)
            {
                return NotFound();
            }

            return View(guest);
        }

        // POST: /Guest/Delete/5  (runs only after the user confirms)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Guest guest = await _context.Guests.FindAsync(id);

            if (guest != null)
            {
                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"{guest.Name} has been removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
