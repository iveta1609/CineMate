using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineMate.Models;

namespace CineMate.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly CineMateDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservationsController(CineMateDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            IQueryable<Reservation> q = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Screening).ThenInclude(s => s.Movie)
                .Include(r => r.Screening).ThenInclude(s => s.Cinema);

            // Клиентът вижда само своите резервации; админ/оператор – всички
            if (User.IsInRole("Client"))
                q = q.Where(r => r.UserId == userId);

            var list = await q.ToListAsync();
            return View(list);
        }

        [Authorize(Roles = "Client,Administrator,Operator")]
        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Screening).ThenInclude(s => s.Movie)
                .Include(r => r.Screening).ThenInclude(s => s.Cinema)
                .Include(r => r.ReservationSeats).ThenInclude(rs => rs.Seat)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null) return NotFound();

            // Клиентите могат да гледат само своите резервации
            if (User.IsInRole("Client") && reservation.UserId != _userManager.GetUserId(User))
                return Forbid();

            // Списък със седалки за View-то
            var seats = reservation.ReservationSeats?
                .Select(rs => rs.Seat)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList() ?? new List<Seat>();

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

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            reservation.UserId = userId;
            reservation.ReservationTime = DateTime.Now;

            // Ако не е подадена сума – пресмятаме по избраните места (ако ги има)
            if (reservation.TotalPrice <= 0 && reservation.ReservationSeats != null && reservation.ReservationSeats.Any())
            {
                reservation.TotalPrice = reservation.ReservationSeats.Sum(rs => rs.Category.GetPrice());
            }

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

        [Authorize]
        public async Task<IActionResult> My(string scope = "upcoming", string paid = "all")
        {
            var userId = _userManager.GetUserId(User);

            // >>> ВАЖНО: q е IQueryable<Reservation>, не var
            IQueryable<Reservation> q = _context.Reservations
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .Include(r => r.Screening).ThenInclude(s => s.Movie)
                .Include(r => r.Screening).ThenInclude(s => s.Cinema);

            var today = DateTime.Today;
            if (scope == "upcoming") q = q.Where(r => r.Screening.StartTime >= today);
            else if (scope == "past") q = q.Where(r => r.Screening.StartTime < today);

            if (paid == "yes") q = q.Where(r => r.IsPaid);
            else if (paid == "no") q = q.Where(r => !r.IsPaid);

            var list = await q.OrderByDescending(r => r.Screening.StartTime).ToListAsync();

            ViewBag.Scope = scope;
            ViewBag.Paid = paid;
            return View(list);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator,Operator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var res = await _context.Reservations
       .Include(r => r.ReservationSeats).ThenInclude(rs => rs.Seat)
       .FirstOrDefaultAsync(r => r.Id == id);

            if (res != null)
            {
                foreach (var rs in res.ReservationSeats)
                    if (rs.Seat != null) rs.Seat.IsAvailable = true;

                _context.ReservationSeats.RemoveRange(res.ReservationSeats);
                _context.Reservations.Remove(res);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await _context.Reservations
                .Include(r => r.Screening)
                .Include(r => r.ReservationSeats).ThenInclude(rs => rs.Seat)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (res == null) return NotFound();

            var isOwner = res.UserId == userId;
            var isAdminOrOp = User.IsInRole("Administrator") || User.IsInRole("Operator");
            if (!isOwner && !isAdminOrOp) return Forbid();

            if (res.IsPaid)
            {
                TempData["Err"] = "Paid reservations cannot be canceled online.";
                return RedirectToAction(nameof(My));
            }

            if (res.Screening?.StartTime <= DateTime.Now)
            {
                TempData["Err"] = "The screening has already started/finished. Cancellation not possible.";
                return RedirectToAction(nameof(My));
            }

            // освобождаваме местата
            foreach (var rs in res.ReservationSeats)
                if (rs.Seat != null) rs.Seat.IsAvailable = true;

            _context.ReservationSeats.RemoveRange(res.ReservationSeats);
            _context.Reservations.Remove(res);
            await _context.SaveChangesAsync();

            TempData["Ok"] = "Reservation canceled. Seats were released.";
            return RedirectToAction(nameof(My), new { scope = "upcoming", paid = "no" });
        }

    }
}
