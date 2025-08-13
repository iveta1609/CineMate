using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CineMate.Data;
using CineMate.Data.Entities;

namespace CineMate.Controllers
{
    // По подразбиране само админ; публичните екшъни са маркирани с [AllowAnonymous]
    [Authorize(Policy = "AdminOnly")]
    [Route("[controller]")]
    public class CinemasController : Controller
    {
        private readonly CineMateDbContext _context;
        public CinemasController(CineMateDbContext context) => _context = context;

        // План за „самоизлекуване“ – какви кина очакваме по град
        private static readonly Dictionary<string, string[]> SeedPlan =
            new(StringComparer.OrdinalIgnoreCase)
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

        // ------------ JSON API: по CityId (първичен път) ------------
        // Абсолютен маршрут -> /Cinemas/ByCity?cityId=1
        [AllowAnonymous]
        [HttpGet("/Cinemas/ByCity")]
        [Produces("application/json")]
        public async Task<IActionResult> ByCity(int cityId)
        {
            if (cityId <= 0)
                return Json(Array.Empty<object>());

            // 1) Опитай да върнеш директно наличните кина
            var list = await _context.Cinemas
                .Where(x => x.CityId == cityId)
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .ToListAsync();

            if (list.Count > 0)
                return Json(list);

            // 2) Няма кина за този CityId -> „самоизлекувай“ според името на града
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
            if (city == null) return Json(Array.Empty<object>());

            await EnsureCinemasForCityId(cityId, city.Name);

            // 3) Върни отново – сега трябва да има
            var result = await _context.Cinemas
                .Where(x => x.CityId == cityId)
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .ToListAsync();

            return Json(result);
        }

        // ------------ JSON API: по име на град (резервен път) ------------
        // /Cinemas/ByCityName?name=Plovdiv
        [AllowAnonymous]
        [HttpGet("/Cinemas/ByCityName")]
        [Produces("application/json")]
        public async Task<IActionResult> ByCityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Json(Array.Empty<object>());

            // Вземаме всички градове със същото име (защото често има дубликати)
            var cities = await _context.Cities
                .Where(c => c.Name.ToLower() == name.ToLower())
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            if (cities.Count == 0)
            {
                // Ако няма такъв град – създай един и му добави кината
                var newCity = new City { Name = name.Trim() };
                _context.Cities.Add(newCity);
                await _context.SaveChangesAsync();

                await EnsureCinemasForCityId(newCity.Id, newCity.Name);
                cities.Add(new { newCity.Id, newCity.Name });
            }
            else
            {
                // За всеки съществуващ град с това име – осигури кината
                foreach (var c in cities)
                    await EnsureCinemasForCityId(c.Id, c.Name);
            }

            // Върни кината за ВСИЧКИ градове с това име
            var ids = cities.Select(c => c.Id).ToList();

            var result = await _context.Cinemas
                .Where(x => ids.Contains(x.CityId))
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .Distinct()
                .ToListAsync();

            return Json(result);
        }

        // Помощник: гарантира, че за конкретен CityId има всички кина от SeedPlan[cityName]
        private async Task EnsureCinemasForCityId(int cityId, string cityName)
        {
            if (!SeedPlan.TryGetValue(cityName ?? string.Empty, out var cinemas))
                return; // ако градът не е в плана – нищо не правим

            bool added = false;
            foreach (var cinemaName in cinemas)
            {
                var exists = await _context.Cinemas
                    .AnyAsync(x => x.CityId == cityId && x.Name == cinemaName);

                if (!exists)
                {
                    _context.Cinemas.Add(new Cinema
                    {
                        Name = cinemaName,
                        CityId = cityId
                    });
                    added = true;
                }
            }
            if (added) await _context.SaveChangesAsync();
        }

        // ------------- CRUD UI -------------

        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                .Include(c => c.City)
                .OrderBy(c => c.City.Name).ThenBy(c => c.Name)
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

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name");
            return View(new Cinema());
        }

        [Authorize(Policy = "OperatorOrAdmin")]
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

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            ViewData["CityId"] = new SelectList(_context.Cities.OrderBy(c => c.Name), "Id", "Name", cinema.CityId);
            return View(cinema);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
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

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _context.Cinemas
                .Include(c => c.City)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
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
