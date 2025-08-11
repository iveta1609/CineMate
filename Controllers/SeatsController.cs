using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using Microsoft.AspNetCore.Authorization;
using CineMate.Data.Entities;

namespace CineMate.Controllers
{
    public class SeatsController : Controller
    {
        private readonly CineMateDbContext _context;
        public SeatsController(CineMateDbContext context)
            => _context = context;

        // GET: Seats
        public async Task<IActionResult> Index()
        {
            var seats = await _context.Seats
                .Include(s => s.Screening).ThenInclude(scr => scr.Movie)
                .Include(s => s.Screening).ThenInclude(scr => scr.Cinema)
                .ToListAsync();
            return View(seats);
        }

        // GET: Seats/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var seat = await _context.Seats
                .Include(s => s.Screening)
                    .ThenInclude(scr => scr.Cinema)
                .Include(s => s.Screening)
                    .ThenInclude(scr => scr.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null) return NotFound();
            return View(seat);
        }

        // GET: Seats/Create
        public IActionResult Create()
        {
            PopulateScreeningDropDown();
            return View();
        }

        // POST: Seats/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seat seat)
        {
            if (ModelState.IsValid)
            {
                seat.IsAvailable = true;
                _context.Seats.Add(seat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // при грешка пак подготвяме dropdown
            PopulateScreeningDropDown(seat.ScreeningId);
            return View(seat);
        }

        // GET: Seats/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat == null) return NotFound();

            PopulateScreeningDropDown(seat.ScreeningId);
            return View(seat);
        }

        // POST: Seats/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seat seat)
        {
            if (id != seat.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Seats.AnyAsync(s => s.Id == id))
                        return NotFound();
                    throw;
                }
            }

            // при грешка пак подготвяме dropdown
            PopulateScreeningDropDown(seat.ScreeningId);
            return View(seat);
        }

        // GET: Seats/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var seat = await _context.Seats
                .Include(s => s.Screening).ThenInclude(scr => scr.Cinema)
                .Include(s => s.Screening).ThenInclude(scr => scr.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null) return NotFound();
            return View(seat);
        }

        // POST: Seats/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seat = await _context.Seats.FindAsync(id);
            if (seat != null)
            {
                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private void PopulateScreeningDropDown(object? selectedScreeningId = null)
        {
            var list = _context.Screenings
                .Include(scr => scr.Cinema)
                .Include(scr => scr.Movie)
                .Select(scr => new
                {
                    scr.Id,
                    Display = $"{scr.Cinema.Name} — {scr.Movie.Title} @ {scr.StartTime:g}"
                })
                .ToList();

            ViewData["ScreeningId"] = new SelectList(list, "Id", "Display", selectedScreeningId);
        }
    }
}
