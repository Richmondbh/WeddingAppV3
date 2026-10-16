using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// Admin pages for the text blocks on the home page (welcome message,
    /// venue, contact and so on). Full create, read, update and delete.
    /// </summary>
    public class WeddingInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeddingInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /WeddingInfo
        public async Task<IActionResult> Index()
        {
            List<WeddingInfo> items = await _context.WeddingInfos
                .OrderBy(w => w.DisplayOrder)
                .ToListAsync();

            return View(items);
        }

        // GET: /WeddingInfo/Create
        public IActionResult Create()
        {
            return View(new WeddingInfo());
        }

        // POST: /WeddingInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Heading,Content,DisplayOrder")] WeddingInfo info)
        {
            if (!ModelState.IsValid)
            {
                return View(info);
            }

            _context.WeddingInfos.Add(info);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"\"{info.Heading}\" was added.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /WeddingInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WeddingInfo info = await _context.WeddingInfos.FindAsync(id);

            if (info == null)
            {
                return NotFound();
            }

            return View(info);
        }

        // POST: /WeddingInfo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Heading,Content,DisplayOrder")] WeddingInfo info)
        {
            if (id != info.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(info);
            }

            _context.WeddingInfos.Update(info);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = $"\"{info.Heading}\" was updated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /WeddingInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            WeddingInfo info = await _context.WeddingInfos.FindAsync(id);

            if (info == null)
            {
                return NotFound();
            }

            return View(info);
        }

        // POST: /WeddingInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            WeddingInfo info = await _context.WeddingInfos.FindAsync(id);

            if (info != null)
            {
                _context.WeddingInfos.Remove(info);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"\"{info.Heading}\" was deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
