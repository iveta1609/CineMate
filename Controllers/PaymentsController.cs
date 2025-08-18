using System;
using System.Linq;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Models.Payments;
using CineMate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Controllers
{
    [AllowAnonymous]
    public class PaymentsController : Controller
    {
        private readonly CineMateDbContext _context;
        private readonly IEmailService _email;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Random _rnd = new Random();

        public PaymentsController(
            CineMateDbContext ctx,
            IEmailService email,
            UserManager<IdentityUser> userManager)
        {
            _context = ctx;
            _email = email;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout(int reservationId)
        {
            var r = await _context.Reservations
                .Include(x => x.Screening).ThenInclude(s => s.Movie)
                .Include(x => x.Screening).ThenInclude(s => s.Cinema)
                .FirstOrDefaultAsync(x => x.Id == reservationId);

            if (r == null) return NotFound();
            if (r.IsPaid) return RedirectToAction("Details", "Reservations", new { id = r.Id });

            var vm = new PaymentCheckoutVm
            {
                ReservationId = r.Id,
                MovieTitle = r.Screening?.Movie?.Title ?? "Movie",
                CinemaName = r.Screening?.Cinema?.Name ?? "Cinema",
                StartTime = r.Screening?.StartTime ?? DateTime.Now,
                Amount = r.TotalPrice
            };

            ViewBag.Form = new PaymentFormVm { ReservationId = r.Id };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PaymentFormVm form)
        {
            var r = await _context.Reservations
                .Include(x => x.Screening).ThenInclude(s => s.Movie)
                .Include(x => x.ReservationSeats).ThenInclude(rs => rs.Seat)
                .FirstOrDefaultAsync(x => x.Id == form.ReservationId);

            if (r == null) return NotFound();
            if (r.IsPaid) return RedirectToAction("Details", "Reservations", new { id = r.Id });

            var digits = new string((form.CardNumber ?? string.Empty).Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(form.Cardholder) ||
                string.IsNullOrWhiteSpace(form.Expiry) ||
                string.IsNullOrWhiteSpace(form.Cvc) ||
                form.Cvc.Trim().Length < 3 ||
                digits.Length != 16 )           
            {
                TempData["Err"] = "Payment failed: please check your card details (16 digits required).";
                return RedirectToAction(nameof(Checkout), new { reservationId = r.Id });
            }

            r.IsPaid = true;
            r.PaidAt = DateTime.UtcNow;
            r.PaymentRef = $"MOCK-{DateTime.UtcNow:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
            await _context.SaveChangesAsync();

            try
            {
                string? to = null;
                if (!string.IsNullOrEmpty(r.UserId))
                {
                    var u = await _userManager.FindByIdAsync(r.UserId);
                    to = u?.Email;
                }
                if (string.IsNullOrWhiteSpace(to) && User?.Identity?.IsAuthenticated == true)
                {
                    var u2 = await _userManager.GetUserAsync(User);
                    to = u2?.Email;
                }
                if (!string.IsNullOrWhiteSpace(to))
                    await _email.SendReservationReceiptAsync(to, r);
            }
            catch {  }

            TempData["Ok"] = $"Payment successful. Ref: {r.PaymentRef}";
            return RedirectToAction("Success", new { reservationId = r.Id });
        }


        public async Task<IActionResult> Success(int reservationId)
        {
            var r = await _context.Reservations.FindAsync(reservationId);
            if (r == null) return NotFound();
            if (!r.IsPaid) return RedirectToAction(nameof(Checkout), new { reservationId });

            ViewBag.ReservationId = r.Id;
            ViewBag.PaymentRef = r.PaymentRef;
            return View();
        }

        private static string OnlyDigits(string s) =>
            new string((s ?? "").Where(char.IsDigit).ToArray());

        private static bool PassesLuhn(string digits)
        {
            if (string.IsNullOrEmpty(digits)) return false;
            int sum = 0; bool alt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int n = digits[i] - '0';
                if (n < 0 || n > 9) return false;
                if (alt) { n *= 2; if (n > 9) n -= 9; }
                sum += n; alt = !alt;
            }
            return sum % 10 == 0;
        }
    }
}
