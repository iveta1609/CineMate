using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineMate.Controllers
{
    // По подразбиране админ; публични са маркирани с [AllowAnonymous]
    [Authorize(Policy = "AdminOnly")]
    [Route("[controller]")]
    public class CinemasController : Controller
    {
        private readonly CineMateDbContext _context;
        public CinemasController(CineMateDbContext context) => _context = context;

        // ========= JSON API =========

        // Абсолютен път, за да няма объркване с route префикса на класа.
        // GET /Cinemas/ByCity?cityId=1 -> [{ id, name }]
        [AllowAnonymous]
        [HttpGet("/Cinemas/ByCity")]
        [Produces("application/json")]
        public async Task<IActionResult> ByCity(int cityId)
        {
            if (cityId <= 0)
                return Json(System.Array.Empty<object>());

            // 1) Първо – кината вързани към точния CityId
            var direct = await _context.Cinemas
        .Where(x => x.CityId == cityId)
        .OrderBy(x => x.Name)
        .Select(x => new { id = x.Id, name = x.Name })
        .ToListAsync();

            if (direct.Count > 0)
                return Json(direct);

            // 2) Няма кина за този CityId -> самовъзстановяване по име на града
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null)
                return Json(Array.Empty<object>());

            // План с кината по град
            var plan = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                ["Sofia"] = new[]
                {
            "CineMate Mall of Sofia",
            "CineMate Paradise Center",
            "CineMate Serdika Center",
            "CineMate The Mall",
            "CineMate Bulgaria Mall"
        },
                ["Plovdiv"] = new[]
                {
            "CineMate Plovdiv Mall",
            "CineMate Markovo Tepe"
        },
                ["Varna"] = new[]
                {
            "CineMate Grand Mall",
            "CineMate Varna Mall"
        }
            };

            // Ако името на града е в плана – добавяме липсващите кина за ТОЧНО този CityId
            if (plan.TryGetValue(city.Name, out var cinemasForCity))
            {
                bool anyAdded = false;

                foreach (var name in cinemasForCity)
                {
                    bool existsHere = await _context.Cinemas
                        .AnyAsync(x => x.CityId == cityId && x.Name == name);

                    if (!existsHere)
                    {
                        _context.Cinemas.Add(new Cinema { Name = name, CityId = cityId });
                        anyAdded = true;
                    }
                }

                if (anyAdded)
                    await _context.SaveChangesAsync();
            }

            // 3) Връщаме резултата след авто-попълване
            var result = await _context.Cinemas
                .Where(x => x.CityId == cityId)
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .ToListAsync();

            return Json(result);
        }

        // ========= CRUD (UI) =========

        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.City)
                .OrderBy(c => c.City.Name)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(cinemas);
        }

        [AllowAnonymous]
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.City)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinema == null) return NotFound();
            return View(cinema);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name");
            return View(new Cinema());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", cinema.CityId);
                return View(cinema);
            }

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", cinema.CityId);
            return View(cinema);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cinema cinema)
        {
            if (id != cinema.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", cinema.CityId);
                return View(cinema);
            }

            try
            {
                _context.Update(cinema);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Cinemas.AnyAsync(e => e.Id == cinema.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema != null)
            {
                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
