using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingAppV3.Data;
using WeddingAppV3.Models;

namespace WeddingAppV3.Controllers
{
    /// <summary>
    /// Public pages that are not about guests or events: the home page,
    /// the photo gallery and the error page.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: / or /Home/Index
        public async Task<IActionResult> Index()
        {
            // The home page text comes from the database and is managed in the admin section.
            List<WeddingInfo> infoBlocks = await _context.WeddingInfos
                .OrderBy(w => w.DisplayOrder)
                .ToListAsync();

            return View(infoBlocks);
        }

        // GET: /Home/Gallery
        public IActionResult Gallery()
        {
            // The pictures are static files in wwwroot/images, so no data is needed.
            return View();
        }

        // GET: /Home/Api
        // A help page that lists the Web API endpoints with clickable links.
        public IActionResult Api()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(model);
        }
    }
}
