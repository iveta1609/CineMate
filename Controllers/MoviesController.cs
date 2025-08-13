using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CineMate.Controllers
{
    [Authorize(Policy = "OperatorOrAdmin")]
    public class MoviesController : Controller
    {
        private readonly CineMateDbContext _context;

        public MoviesController(CineMateDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string q)
        {
            var movies = _context.Movies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                var like = $"%{q}%";
                movies = movies.Where(m => EF.Functions.Like(m.Title, like));
                ViewBag.Query = q; 
            }

            var list = await movies
                .OrderBy(m => m.Title)
                .ToListAsync();

            return View(list); 
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [AllowAnonymous]
        public async Task<IActionResult> DetailsByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return NotFound();

            var movie = await _context.Movies
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Title == title);

            if (movie == null) return NotFound();

            return RedirectToAction(nameof(Details), new { id = movie.Id });
        }


        [Authorize(Policy = "OperatorOrAdmin")]
        public IActionResult Create()
        {
            return View(new Movie());
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Director,ReleaseYear,Genre,Synopsis,DurationMinutes,MainActors")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Director,ReleaseYear,Genre,Synopsis,DurationMinutes,MainActors")] Movie movie)
        {
            if (id != movie.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Movies.AnyAsync(m => m.Id == id))
                        return NotFound();
                    throw;
                }
            }
            return View(movie);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
