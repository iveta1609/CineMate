using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineMate.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("[controller]")]
    public class CinemasController : Controller
    {
        private readonly CineMateDbContext _context;
        public CinemasController(CineMateDbContext context) => _context = context;

        // GET /Cinemas or /Cinemas/Index
        [AllowAnonymous]
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var cinemas = await _context.Cinemas
                                        .Include(c => c.City)
                                        .ToListAsync();
            return View(cinemas);
        }

        // GET /Cinemas/Details/5
        [AllowAnonymous]
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _context.Cinemas
                                       .Include(c => c.City)
                                       .FirstOrDefaultAsync(c => c.Id == id);
            if (cinema == null) return NotFound();
            return View(cinema);
        }

        // GET /Cinemas/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            return View(new Cinema());
        }

        // POST /Cinemas/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cinema cinema)
        {
            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // TODO: Edit, Delete actions inherit controller-level [Authorize]
    }
}