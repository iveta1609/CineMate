namespace CineMate.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ScreeningId { get; set; }
        public string SeatIdsCsv { get; set; } = ""; 
        public string Category { get; set; } = "Adult";
        public decimal UnitPrice { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice { get; set; }
        public int? ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Screening? Screening { get; set; }
    }
}
