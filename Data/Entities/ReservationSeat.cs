using CineMate.Models;

namespace CineMate.Data.Entities
{
    namespace CineMate.Data.Entities
    {
        public class ReservationSeat
        {
            public int ReservationId { get; set; }
            public Reservation Reservation { get; set; } = null!;

            public int SeatId { get; set; }
            public Seat Seat { get; set; } = null!;
            public string CategoryName { get; set; } = null!;
            public decimal CategoryPrice { get; set; }

            public TicketCategory Category { get; set; }

        }
    }

}
