using System;

namespace CineMate.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;
        public DateTime PaidAt { get; set; }
        public string TransactionId { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
