using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CineMate.Data;
using CineMate.Data.Entities;

namespace CineMate.Controllers
{
    // Админ по подразбиране; публични са екшъните със [AllowAnonymous]
    [Authorize(Policy = "AdminOnly")]
    [Route("[controller]")]
    public class CitiesController : Controller
    {
        private readonly CineMateDbContext _context;
        public CitiesController(CineMateDbContext context) => _context = context;

        // JSON за хедъра: GET /Cities/List -> [{ id, name }]
        [AllowAnonymous]
        [HttpGet("/Cities/List")]
        [Produces("application/json")]
        public async Task<IActionResult> List()
        {
            var data = await _context.Cities
                .OrderBy(c => c.Name)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();

            return Json(data);
        }

        // UI списък
        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(cities);
        }

        // UI детална
        [AllowAnonymous]
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return View(city);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Create")]
        public IActionResult Create() => View();

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] City city)
        {
            if (!ModelState.IsValid) return View(city);

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null) return NotFound();
            return View(city);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] City city)
        {
            if (id != city.Id) return BadRequest();

            if (!ModelState.IsValid) return View(city);

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

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpGet("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
            if (city == null) return NotFound();
            return View(city);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost("Delete/{id:int}")]
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
