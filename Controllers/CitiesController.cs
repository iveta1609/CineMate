using System.Linq;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CitiesController : Controller
    {
        private readonly CineMateDbContext _context;
        public CitiesController(CineMateDbContext context) => _context = context;

        [AllowAnonymous]
        [HttpGet("/Cities/List")]
        public async Task<IActionResult> List()
        {
            var list = await _context.Cities.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();
            return Json(list);
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.Cities.AsNoTracking()
                .OrderBy(c => c.Name).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var city = await _context.Cities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return View(city);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] City city)
        {
            if (id != city.Id) return NotFound();
            if (!ModelState.IsValid) return View(city);
            _context.Update(city);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
