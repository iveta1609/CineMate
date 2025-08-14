using System.Linq;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    public class MoviesController : Controller
    {
        private readonly CineMateDbContext _context;
        public MoviesController(CineMateDbContext ctx) => _context = ctx;

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search)
        {
            var q = _context.Movies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(m =>
                    (m.Title ?? "").ToLower().Contains(s) ||
                    (m.Director ?? "").ToLower().Contains(s));
            }

            var list = await q.OrderBy(m => m.Title).ToListAsync();
            return View(list);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();
            return View(movie);
        }

        [AllowAnonymous]
        [HttpGet("DetailsByTitle")]
        public async Task<IActionResult> DetailsByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return RedirectToAction(nameof(Index));

            var t = title.Trim().ToLower();

            // 1) точен мач (без чувствителност към регистър)
            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Title.ToLower() == t);

            // 2) ако няма – търсене по съдържание
            if (movie == null)
            {
                movie = await _context.Movies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Title.ToLower().Contains(t));
            }

            if (movie == null)
                return NotFound(); // по желание може да върнеш View с "не е намерен"

            // пренасочваме към нормалната страница за детайли
            return RedirectToAction(nameof(Details), new { id = movie.Id });
        }

        [Authorize(Roles = "Administrator,Operator")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Administrator,Operator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (!ModelState.IsValid) return View(movie);

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================

        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Operator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id) return BadRequest();
            if (!ModelState.IsValid) return View(movie);

            _context.Update(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===================== DELETE =====================

        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator,Operator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Screenings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return RedirectToAction(nameof(Index));

            // Ако има прожекции към филма – чистим зависимостите
            if (movie.Screenings != null && movie.Screenings.Any())
            {
                var screeningIds = movie.Screenings.Select(s => s.Id).ToList();

                // Резервации + техните седалки
                var reservations = await _context.Reservations
                    .Include(r => r.ReservationSeats).ThenInclude(rs => rs.Seat)
                    .Where(r => screeningIds.Contains(r.ScreeningId))
                    .ToListAsync();

                // Освобождаваме седалките
                foreach (var r in reservations)
                    foreach (var rs in r.ReservationSeats)
                        if (rs.Seat != null) rs.Seat.IsAvailable = true;

                _context.ReservationSeats.RemoveRange(reservations.SelectMany(r => r.ReservationSeats));
                _context.Reservations.RemoveRange(reservations);

                // Седалки към прожекциите
                var seats = await _context.Seats
                    .Where(s => screeningIds.Contains(s.ScreeningId))
                    .ToListAsync();
                _context.Seats.RemoveRange(seats);

                // Самите прожекции
                _context.Screenings.RemoveRange(movie.Screenings);
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Movie deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
