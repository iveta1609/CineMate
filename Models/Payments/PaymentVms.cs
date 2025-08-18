namespace CineMate.Models.Payments
{
    public class PaymentCheckoutVm
    {
        public int ReservationId { get; set; }
        public string MovieTitle { get; set; } = "";
        public string CinemaName { get; set; } = "";
        public DateTime StartTime { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentFormVm
    {
        public int ReservationId { get; set; }
        public string Cardholder { get; set; } = "";
        public string CardNumber { get; set; } = "";
        public string Expiry { get; set; } = ""; 
        public string Cvc { get; set; } = "";
    }
}
