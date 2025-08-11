using CineMate.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace CineMate.Models
{
    public class ReservationDetailsViewModel
    {
        public Reservation Reservation { get; set; } = null!;
        public List<Seat> Seats { get; set; }


        public int TotalTickets => Reservation.ReservationSeats.Count;

        public Dictionary<string, int> TicketsByCategory =>
            Reservation.ReservationSeats
                .GroupBy(rs => rs.Category.DisplayName())
                .ToDictionary(g => g.Key, g => g.Count());

        public decimal ComputedTotalPrice =>
            Reservation.ReservationSeats.Sum(rs => rs.Category.GetPrice());

        public string SeatsDisplay =>
            string.Join(", ", Reservation.ReservationSeats
                .OrderBy(rs => rs.Seat.Row)
                .ThenBy(rs => rs.Seat.Number)
                .Select(rs => $"{rs.Seat.Row}-{rs.Seat.Number} ({rs.Category.DisplayName()})"));
    }
}
