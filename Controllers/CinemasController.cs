using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CinemasController : Controller
    {
        private readonly CineMateDbContext _context;
        public CinemasController(CineMateDbContext context) => _context = context;


        [AllowAnonymous]
        [HttpGet("/Cinemas/ByCity")]
        public async Task<IActionResult> ByCity(int cityId)
        {
            if (cityId <= 0) return Json(Array.Empty<object>());
            var list = await _context.Cinemas
                .AsNoTracking()
                .Where(x => x.CityId == cityId)
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .ToListAsync();
            return Json(list);
        }

        [AllowAnonymous]
        [HttpGet("/Cinemas/ByCityName")]
        public async Task<IActionResult> ByCityName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Json(Array.Empty<object>());
            var ids = await _context.Cities.AsNoTracking()
                .Where(c => c.Name.ToLower() == name.ToLower())
                .Select(c => c.Id).ToListAsync();

            var list = await _context.Cinemas.AsNoTracking()
                .Where(x => ids.Contains(x.CityId))
                .OrderBy(x => x.Name)
                .Select(x => new { id = x.Id, name = x.Name })
                .Distinct()
                .ToListAsync();

            return Json(list);
        }


        public async Task<IActionResult> Index()
        {
            var list = await _context.Cinemas.AsNoTracking()
                .Include(c => c.City)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas.AsNoTracking()
                .Include(c => c.City)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _context.Cinemas.FindAsync(id);
            if (cinema == null) return NotFound();

            ViewBag.CityId = new SelectList(
                await _context.Cities.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                nameof(City.Id), nameof(City.Name), cinema.CityId);
            return View(cinema);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CityId")] Cinema cinema)
        {
            if (id != cinema.Id) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewBag.CityId = new SelectList(
                    await _context.Cities.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                    nameof(City.Id), nameof(City.Name), cinema.CityId);
                return View(cinema);
            }
            _context.Update(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
