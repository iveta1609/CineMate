using CineMate.Data.Entities.CineMate.Data.Entities;

namespace CineMate.Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int ScreeningId { get; set; }
        public Screening Screening { get; set; } = null!;

        public DateTime ReservationTime { get; set; }

        public decimal TotalPrice { get; set; }

        public ICollection<ReservationSeat> ReservationSeats { get; set; } = new List<ReservationSeat>();

        public Payment? Payment { get; set; }
        public List<Seat> Seats { get; set; }

        public string Category { get; set; } = null!;
    }
}
