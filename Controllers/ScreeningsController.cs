using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
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

        // everyone
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
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

        // operator + admin
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

            // seed seats
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
            var screening = await _context.Screenings.FindAsync(id);
            if (screening != null)
            {
                _context.Screenings.Remove(screening);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        //
        // === STEP 1: pick your seats (no pricing) ===
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
        public async Task<IActionResult> SelectSeats(int id, int[] selectedSeatIds, string SelectedCategory)
        {
            // 1) Валидиране
            if (selectedSeatIds == null || selectedSeatIds.Length == 0)
                ModelState.AddModelError("selectedSeatIds", "You must select at least one seat.");

            if (string.IsNullOrWhiteSpace(SelectedCategory))
                ModelState.AddModelError("SelectedCategory", "Please choose a ticket category.");

            if (!ModelState.IsValid)
            {
                // Презареждаме VM с всички нужни списъци
                var vm = await BuildSelectSeatsVm(id);
                vm.SelectedCategory = SelectedCategory;
                return View(vm);
            }

            // 2) Изчисляваме цена на билет спрямо категорията
            decimal unitPrice = SelectedCategory switch
            {
                "Adult" => 16m,
                "Teen" => 11m,
                "Kids" => 7m,
                _ => 16m
            };

            // 3) Вземаме userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 4) Създаваме резервация
            var reservation = new Reservation
            {
                ScreeningId = id,
                UserId = userId,
                Category = SelectedCategory,
                TotalPrice = unitPrice * selectedSeatIds.Length,
                ReservationTime = DateTime.Now
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // 5) Обновяваме статус на местата
            var seats = await _context.Seats
                .Where(s => selectedSeatIds.Contains(s.Id)).ToListAsync();
            seats.ForEach(s => s.IsAvailable = false);
            await _context.SaveChangesAsync();

            // 6) Пренасочваме към страницата с детайли на резервацията
            return RedirectToAction("Details", "Reservations", new { id = reservation.Id });
        }

        // Помощен метод за презареждане на VM
        private async Task<SelectSeatsViewModel> BuildSelectSeatsVm(int id)
        {
            var screening = await _context.Screenings
                .Include(s => s.Cinema)
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new InvalidOperationException("Screening not found");

            var seats = await _context.Seats
                .Where(s => s.ScreeningId == id)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToListAsync();

            return new SelectSeatsViewModel
            {
                Screening = screening,
                Seats = seats,
                Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 лв.)", "Adult"),
                    new SelectListItem("Teen (11 лв.)", "Teen"),
                    new SelectListItem("Kids (7 лв.)", "Kids"),
                }
            };
        }
            
        private async Task<SelectSeatsViewModel> BuildSelectSeatsVm(int id, SelectSeatsViewModel vmOld = null)
        {
            var screening = await _context.Screenings
                .Include(s => s.Movie)
                .Include(s => s.Cinema)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (screening == null) return null;

            var seats = await _context.Seats
                .Where(s => s.ScreeningId == id)
                .OrderBy(s => s.Row).ThenBy(s => s.Number)
                .ToListAsync();

            var vm = new SelectSeatsViewModel
            {
                Screening = screening,
                Seats = seats,
                Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 лв.)", "Adult"),
                    new SelectListItem("Teen (11 лв.)", "Teen"),
                    new SelectListItem("Kids (7 лв.)", "Kids"),
                }
            };

            // ако идва от неуспешен POST, възстановяваме избора
            if (vmOld != null)
            {
                vm.SelectedSeatIds = vmOld.SelectedSeatIds;
                vm.SelectedCategory = vmOld.SelectedCategory;
            }

            return vm;
        }

        private async Task<SelectSeatsViewModel> BuildSelectSeatsVm(int id)
        {
            var screening = await _context.Screenings
                                       .Include(s => s.Movie)
                                       .Include(s => s.Cinema)
                                       .Include(s => s.Seats)
                                       .FirstOrDefaultAsync(s => s.Id == id);

            var seats = screening.Seats.OrderBy(s => s.Row).ThenBy(s => s.Number).ToList();

            return new SelectSeatsViewModel
            {
                Screening = screening,
                Seats = seats,
                Categories = new List<SelectListItem> {
            new("Adult (16 лв.)", "Adult"),
            new("Teen (11 лв.)", "Teen"),
            new("Kids (7 лв.)", "Kids"),
        }
            };
        }


        //
        // === STEP 2: choose category + confirm seats + price ===
        [Authorize(Roles = "Client,Administrator")]
        [HttpGet]
        public async Task<IActionResult> ReserveSeats(int id, int[] selectedSeatIds)
        {
            // load the reservation + screening + seats
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
                    new SelectListItem("Adult (16 лв.)", "Adult"),
                    new SelectListItem("Teen  (11 лв.)", "Teen"),
                    new SelectListItem("Kids  ( 7 лв.)", "Kids"),
                }
            };
            return View(vm);
        }
        

        [Authorize(Roles = "Client,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReserveSeats(ReserveSeatsViewModel vm)
        {
            // validate seats
            if (vm.SelectedSeatIds == null || vm.SelectedSeatIds.Length == 0)
                ModelState.AddModelError(nameof(vm.SelectedSeatIds), "You must select at least one seat.");

            // validate category
            if (string.IsNullOrEmpty(vm.SelectedCategory))
                ModelState.AddModelError(nameof(vm.SelectedCategory), "Please choose a ticket category.");

            if (!ModelState.IsValid)
            {
                // reload for redisplay
                var screening = await _context.Screenings
                    .Include(s => s.Seats)
                    .FirstOrDefaultAsync(s => s.Id == vm.ScreeningId);
                vm.Screening = screening;
                vm.Seats = screening.Seats
                    .Where(s => vm.SelectedSeatIds.Contains(s.Id))
                    .OrderBy(s => s.Row).ThenBy(s => s.Number)
                    .ToList();
                vm.Categories = new List<SelectListItem>
                {
                    new SelectListItem("Adult (16 лв.)", "Adult"),
                    new SelectListItem("Teen  (11 лв.)", "Teen"),
                    new SelectListItem("Kids  ( 7 лв.)", "Kids"),
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

            var reservation = new Reservation
            {
                ScreeningId = vm.ScreeningId,
                UserId = user.Id,
                Category = vm.SelectedCategory,
                TotalPrice = vm.SelectedSeatIds.Length * unitPrice,
                ReservationTime = DateTime.Now
            };
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // mark those seats as taken
            var seats = await _context.Seats
                .Where(s => vm.SelectedSeatIds.Contains(s.Id))
                .ToListAsync();
            seats.ForEach(s => s.IsAvailable = false);
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
