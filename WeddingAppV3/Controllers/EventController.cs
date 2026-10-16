using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// The public Event page (Index) and the admin pages for creating,
    /// editing and deleting the events of the wedding day.
    /// </summary>
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Event  (public page)
        public async Task<IActionResult> Index()
        {
            List<Event> events = await _context.Events.OrderBy(e => e.Id).ToListAsync();
            return View(events);
        }

        // GET: /Event/Manage  (admin list with edit and delete)
        public async Task<IActionResult> Manage()
        {
            List<Event> events = await _context.Events.OrderBy(e => e.Id).ToListAsync();
            return View(events);
        }

        // GET: /Event/Create
        public IActionResult Create()
        {
            return View(new Event());
        }

        // POST: /Event/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Date,Time,Location,Description")] Event weddingEvent)
        {
            if (!ModelState.IsValid)
            {
                return View(weddingEvent);
            }

            _context.Events.Add(weddingEvent);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"\"{weddingEvent.Title}\" was added.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: /Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event weddingEvent = await _context.Events.FindAsync(id);

            if (weddingEvent == null)
            {
                return NotFound();
            }

            return View(weddingEvent);
        }

        // POST: /Event/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Title,Date,Time,Location,Description")] Event weddingEvent)
        {
            if (id != weddingEvent.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(weddingEvent);
            }

            // Every column comes from the form, so the whole entity can be updated.
            _context.Events.Update(weddingEvent);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"\"{weddingEvent.Title}\" was updated.";
            return RedirectToAction(nameof(Manage));
        }

        // GET: /Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event weddingEvent = await _context.Events.FindAsync(id);

            if (weddingEvent == null)
            {
                return NotFound();
            }

            return View(weddingEvent);
        }

        // POST: /Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Event weddingEvent = await _context.Events.FindAsync(id);

            if (weddingEvent != null)
            {
                _context.Events.Remove(weddingEvent);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"\"{weddingEvent.Title}\" was deleted.";
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}
