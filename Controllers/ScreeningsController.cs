using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using CineMate.Data.Entities.CineMate.Data.Entities;
using CineMate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    public class ScreeningsController : Controller
    {
        private readonly CineMateDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ScreeningsController(
            CineMateDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? cityId, int? cinemaId)
        {
            IQueryable<Screening> q = _context.Screenings
                .AsNoTracking()
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                    .ThenInclude(c => c.City);

            if (cityId.HasValue)
                q = q.Where(s => s.Cinema.CityId == cityId.Value);

            if (cinemaId.HasValue)
                q = q.Where(s => s.CinemaId == cinemaId.Value);

            var list = await q
                .Where(s => s.StartTime >= DateTime.Today)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            return View(list);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null) return NotFound();
            return View(screening);
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        public IActionResult Create()
        {
            PopulateDropDowns();
            return View(new Screening { StartTime = DateTime.Now });
        }

        [HttpPost]
        [Authorize(Policy = "OperatorOrAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Screening screening)
        {
            ModelState.Remove(nameof(screening.Cinema));
            ModelState.Remove(nameof(screening.Movie));

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(screening.CinemaId, screening.MovieId);
                return View(screening);
            }

            _context.Screenings.Add(screening);
            await _context.SaveChangesAsync();

            var seats = new List<Seat>();
            for (int r = 1; r <= 7; r++)
                for (int n = 1; n <= 10; n++)
                    seats.Add(new Seat
                    {
                        ScreeningId = screening.Id,
                        Row = r,
                        Number = n,
                        IsAvailable = true
                    });
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var screening = await _context.Screenings.FindAsync(id);
            if (screening == null) return NotFound();
            PopulateDropDowns(screening.CinemaId, screening.MovieId);
            return View(screening);
        }

        [HttpPost]
        [Authorize(Policy = "OperatorOrAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Screening screening)
        {
            if (id != screening.Id) return BadRequest();

            ModelState.Remove(nameof(screening.Cinema));
            ModelState.Remove(nameof(screening.Movie));

            if (!ModelState.IsValid)
            {
                PopulateDropDowns(screening.CinemaId, screening.MovieId);
                return View(screening);
            }

            _context.Update(screening);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "OperatorOrAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null) return NotFound();
            return View(screening);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "OperatorOrAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screening != null)
            {
                var reservations = await _context.Reservations
                    .Include(r => r.ReservationSeats)
                        .ThenInclude(rs => rs.Seat)
                    .Where(r => r.ScreeningId == id)
                    .ToListAsync();

                foreach (var r in reservations)
                {
                    foreach (var rs in r.ReservationSeats)
                        if (rs.Seat != null) rs.Seat.IsAvailable = true;
                }

                _context.ReservationSeats.RemoveRange(
                    reservations.SelectMany(r => r.ReservationSeats));
                _context.Reservations.RemoveRange(reservations);

                _context.Seats.RemoveRange(screening.Seats);
                _context.Screenings.Remove(screening);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SelectSeats(int id)
        {
            var vm = await BuildSelectSeatsVm(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [Authorize(Policy = "ClientOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectSeats(
            int id,
            int[]? SelectedSeatIds,
            string? SelectedCategory,
            string mode) 
        {
            var chosenSeatIds = SelectedSeatIds ?? Array.Empty<int>();

            if (chosenSeatIds.Length == 0)
                ModelState.AddModelError(nameof(SelectSeatsViewModel.SelectedSeatIds), "You must select at least one seat.");

            if (string.IsNullOrWhiteSpace(SelectedCategory))
                ModelState.AddModelError(nameof(SelectSeatsViewModel.SelectedCategory), "Please choose a ticket category.");

            if (!ModelState.IsValid)
            {
                var vmInvalid = await BuildSelectSeatsVm(id);
                if (vmInvalid == null) return NotFound();
                vmInvalid.SelectedSeatIds = chosenSeatIds;
                vmInvalid.SelectedCategory = SelectedCategory;
                return View(vmInvalid);
            }

            decimal unitPrice = SelectedCategory switch
            {
                "Adult" => 16m,
                "Teen" => 11m,
                "Kids" => 7m,
                _ => 16m
            };

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            if (string.Equals(mode, "cart", StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _context.Seats
                    .Where(s => s.ScreeningId == id && chosenSeatIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync();

                if (exists.Count != chosenSeatIds.Length)
                {
                    TempData["Err"] = "Some seats are invalid. Please try again.";
                    return RedirectToAction(nameof(SelectSeats), new { id });
                }

                var item = new CartItem
                {
                    UserId = userId,
                    ScreeningId = id,
                    SeatIdsCsv = string.Join(',', chosenSeatIds),
                    Category = SelectedCategory!,
                    TotalPrice = unitPrice * chosenSeatIds.Length,
                    AddedAt = DateTime.UtcNow
                };
                _context.CartItems.Add(item);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Cart");
            }

            var seatsToTake = await _context.Seats
                .Where(s => s.ScreeningId == id && chosenSeatIds.Contains(s.Id))
                .ToListAsync();

            if (seatsToTake.Count != chosenSeatIds.Length || seatsToTake.Any(s => !s.IsAvailable))
            {
                ModelState.AddModelError(string.Empty, "One or more selected seats are no longer available. Please choose again.");
                var vmRetry = await BuildSelectSeatsVm(id);
                if (vmRetry == null) return NotFound();
                vmRetry.SelectedSeatIds = seatsToTake.Where(s => s.IsAvailable).Select(s => s.Id).ToArray();
                vmRetry.SelectedCategory = SelectedCategory;
                return View(vmRetry);
            }

            var reservation = new Reservation
            {
                ScreeningId = id,
                UserId = userId,
                Category = SelectedCategory!,
                TotalPrice = unitPrice * seatsToTake.Count,
                ReservationTime = DateTime.Now
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            _context.ReservationSeats.AddRange(seatsToTake.Select(s => new ReservationSeat
            {
                ReservationId = reservation.Id,
                SeatId = s.Id,
                CategoryName = SelectedCategory!
            }));
            seatsToTake.ForEach(s => s.IsAvailable = false);
            await _context.SaveChangesAsync();

            return RedirectToAction("Checkout", "Payments", new { reservationId = reservation.Id });
        }

        private async Task<SelectSeatsViewModel?> BuildSelectSeatsVm(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (screening == null) return null;

            await EnsureSeatsForScreeningAsync(screening, 7, 10);

            var seats = screening.Seats
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Number)
                .ToList();

            return new SelectSeatsViewModel
            {
                ScreeningId = id,
                Screening = screening,
                Seats = seats,
                Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 BGN)", "Adult"),
                    new SelectListItem("Teen (11 BGN)",  "Teen"),
                    new SelectListItem("Kids (7 BGN)",   "Kids"),
                }
            };
        }

        private async Task EnsureSeatsForScreeningAsync(Screening screening, int defaultRows, int defaultCols)
        {
            if (screening.Seats != null && screening.Seats.Any())
                return;

            var toAdd = new List<Seat>();
            for (int r = 1; r <= defaultRows; r++)
                for (int c = 1; c <= defaultCols; c++)
                    toAdd.Add(new Seat
                    {
                        ScreeningId = screening.Id,
                        Row = r,
                        Number = c,
                        IsAvailable = true
                    });

            _context.Seats.AddRange(toAdd);
            await _context.SaveChangesAsync();

            await _context.Entry(screening).Collection(s => s.Seats).LoadAsync();
        }

        [Authorize(Roles = "Client,Administrator")]
        [HttpGet]
        public async Task<IActionResult> ReserveSeats(int id, int[] selectedSeatIds)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
                .Include(s => s.Seats)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null) return NotFound();

            var seats = screening.Seats
                .Where(s => selectedSeatIds.Contains(s.Id))
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToList();

            var vm = new ReserveSeatsViewModel
            {
                Screening = screening,
                ScreeningId = id,
                Seats = seats,
                SelectedSeatIds = selectedSeatIds ?? Array.Empty<int>(),
                Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 BGN)", "Adult"),
                    new SelectListItem("Teen (11 BGN)",  "Teen"),
                    new SelectListItem("Kids (7 BGN)",   "Kids"),
                }
            };
            return View(vm);
        }

        [Authorize(Roles = "Client,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReserveSeats(ReserveSeatsViewModel vm)
        {
            if (vm.SelectedSeatIds == null || vm.SelectedSeatIds.Length == 0)
                ModelState.AddModelError(nameof(vm.SelectedSeatIds), "You must select at least one seat.");

            if (string.IsNullOrEmpty(vm.SelectedCategory))
                ModelState.AddModelError(nameof(vm.SelectedCategory), "Please choose a ticket category.");

            if (!ModelState.IsValid)
            {
                var screeningReload = await _context.Screenings
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == vm.ScreeningId);

                vm.Screening = screeningReload;
                vm.Seats = screeningReload.Seats
                    .Where(s => vm.SelectedSeatIds.Contains(s.Id))
                    .OrderBy(s => s.Row).ThenBy(s => s.Number)
                    .ToList();
                vm.Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 BGN)", "Adult"),
                    new SelectListItem("Teen (11 BGN)",  "Teen"),
                    new SelectListItem("Kids (7 BGN)",   "Kids"),
                };
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            decimal unitPrice = vm.SelectedCategory switch
            {
                "Adult" => 16m,
                "Teen" => 11m,
                "Kids" => 7m,
                _ => 16m
            };

            var seatsToTake = await _context.Seats
                .Where(s => s.ScreeningId == vm.ScreeningId && vm.SelectedSeatIds.Contains(s.Id))
                .ToListAsync();

            if (seatsToTake.Count != vm.SelectedSeatIds.Length || seatsToTake.Any(s => !s.IsAvailable))
            {
                ModelState.AddModelError(string.Empty, "One or more selected seats are no longer available. Please choose again.");
                var screening = await _context.Screenings
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == vm.ScreeningId);
                vm.Screening = screening;
                vm.Seats = seatsToTake.Where(s => s.IsAvailable)
                    .OrderBy(s => s.Row).ThenBy(s => s.Number)
                    .ToList();
                vm.Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 BGN)", "Adult"),
                    new SelectListItem("Teen (11 BGN)",  "Teen"),
                    new SelectListItem("Kids (7 BGN)",   "Kids"),
                };
                return View(vm);
            }

            var reservation = new Reservation
            {
                ScreeningId = vm.ScreeningId,
                UserId = user.Id,
                Category = vm.SelectedCategory,
                TotalPrice = seatsToTake.Count * unitPrice,
                ReservationTime = DateTime.Now
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            seatsToTake.ForEach(s => s.IsAvailable = false);
            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { reservationId = reservation.Id });
        }

        [Authorize(Roles = "Client,Administrator")]
        public async Task<IActionResult> Confirmation(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Movie)
                .Include(r => r.Screening)
                    .ThenInclude(s => s.Cinema)
                .FirstOrDefaultAsync(r => r.Id == reservationId);
            return View(reservation);
        }

        private void PopulateDropDowns(int? cin = null, int? mov = null)
        {
            ViewData["CinemaId"] = new SelectList(
                _context.Cinemas.OrderBy(c => c.Name),
                "Id", "Name", cin);
            ViewData["MovieId"] = new SelectList(
                _context.Movies.OrderBy(m => m.Title),
                "Id", "Title", mov);
        }
    }
}
