using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using Microsoft.AspNetCore.Authorization;
using CineMate.Data.Entities;

namespace CineMate.Controllers
{
    [Authorize(Policy = "AdminOnly")]

    public class CitiesController : Controller
    {
        private readonly CineMateDbContext _context;

        public CitiesController(CineMateDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities.ToListAsync();
            return View(cities);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return View(city);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] City city)
        {
            if (ModelState.IsValid)
            {
                _context.Add(city);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            if (id != city.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Cities.AnyAsync(c => c.Id == id))
                        return NotFound();
                    throw;
                }
            }
            return View(city);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var city = await _context.Cities
                .FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return View(city);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
