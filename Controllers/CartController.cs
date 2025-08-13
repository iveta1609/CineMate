using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using CineMate.Data.Entities.CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CineMateDbContext _context;

        public CartController(CineMateDbContext ctx) => _context = ctx;

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.CartItems
                .Include(ci => ci.Screening).ThenInclude(s => s.Movie)
                .Include(ci => ci.Screening).ThenInclude(s => s.Cinema)
                .Where(ci => ci.UserId == uid)
                .OrderByDescending(ci => ci.AddedAt)
                .ToListAsync();

            ViewBag.Total = items.Sum(i => i.TotalPrice);
            return View(items);
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReserveAndPay(int id)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uid)) return Challenge();

            var item = await _context.CartItems
                .Include(x => x.Screening).ThenInclude(s => s.Seats)
                .Include(x => x.Screening).ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

            if (item == null)
            {
                TempData["Err"] = "Cart item not found.";
                return RedirectToAction(nameof(Index));
            }

            // seat ids от кошницата
            var seatIds = (item.SeatIdsCsv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .ToArray();

            // проверка за наличност
            var seatsToTake = await _context.Seats
                .Where(s => s.ScreeningId == item.ScreeningId && seatIds.Contains(s.Id))
                .ToListAsync();

            if (seatsToTake.Count != seatIds.Length || seatsToTake.Any(s => !s.IsAvailable))
            {
                TempData["Err"] = "Some seats are no longer available.";
                return RedirectToAction(nameof(Index));
            }

            // създай Reservation
            var reservation = new Reservation
            {
                ScreeningId = item.ScreeningId,
                UserId = uid,
                Category = item.Category,
                TotalPrice = item.TotalPrice,
                ReservationTime = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // свържи местата
            _context.ReservationSeats.AddRange(seatsToTake.Select(s => new ReservationSeat
            {
                ReservationId = reservation.Id,
                SeatId = s.Id,
                CategoryName = item.Category
            }));

            // маркирай местата като заети
            seatsToTake.ForEach(s => s.IsAvailable = false);

            // махни артикула от кошницата
            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            // към плащане
            return RedirectToAction("Checkout", "Payments", new { reservationId = reservation.Id });
        }
    }
}
