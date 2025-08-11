using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using CineMate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly CineMateDbContext _context;

        public ReservationsController(CineMateDbContext context)
            => _context = context;

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Reservations
                                .Include(r => r.Screening)
                                    .ThenInclude(s => s.Movie)
                                .Include(r => r.Screening)
                                    .ThenInclude(s => s.Cinema)
                                .AsQueryable();

            if (User.IsInRole("Client"))
                query = query.Where(r => r.UserId == userId);

            var list = await query.ToListAsync();
            return View(list);
        }

        [Authorize(Roles = "Client,Administrator")]
        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _context.Reservations
                  .Include(r => r.Screening)
                      .ThenInclude(s => s.Movie)
                  .Include(r => r.Screening)
                      .ThenInclude(s => s.Cinema)
                  .Include(r => r.ReservationSeats)
                      .ThenInclude(rs => rs.Seat)
                  .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();

            return View(reservation);

            // Създаваме списък от Seat обекти за view-а
            var seats = reservation.ReservationSeats
                .Select(rs => rs.Seat)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList();

            ViewData["SeatList"] = seats;
            return View(reservation);
        }
    


// GET: Reservations/Create?screeningId=5
        [Authorize(Roles = "Client")]
        public IActionResult Create(int screeningId)
        {
            var reservation = new Reservation
            {
                ScreeningId = screeningId
            };
            return View(reservation);
        }

        // POST: Reservations/Create
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            if (!ModelState.IsValid) return View(reservation);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();
            reservation.ReservationTime = System.DateTime.Now;

            // Ако нямаш избрани конкретни места тук, остави TotalPrice 0 или го пресметни по логика
            reservation.TotalPrice = reservation.ReservationSeats.Sum(rs => rs.Category.GetPrice());

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = reservation.Id });
        }

        [Authorize(Roles = "Administrator,Operator")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await _context.Reservations
                .Include(r => r.Screening).ThenInclude(s => s.Movie)
                .Include(r => r.Screening).ThenInclude(s => s.Cinema)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return NotFound();
            return View(res);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator,Operator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var res = await _context.Reservations
                .Include(r => r.ReservationSeats)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (res != null)
            {
                // освобождаваме местата (ако маркираш при резервация)
                _context.Reservations.Remove(res);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
