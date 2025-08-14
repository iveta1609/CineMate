namespace CineMate.Models
{
    public class ScreeningCardVm
    {
        public int ScreeningId { get; set; }
        public string MovieTitle { get; set; } = "";
        public string PosterUrl { get; set; } = "";
        public DateTime StartTime { get; set; }
        public string HallName { get; set; } = "";
        public string City { get; set; } = "";
        public string CinemaName { get; set; } = "";
     
        public int RuntimeMin { get; set; }        // optional
        public decimal? Price { get; set; }        // optional
    }

    public class ScreeningsDayVm
    {
        public DateTime Date { get; set; }
        public List<ScreeningCardVm> Items { get; set; } = new();
    }

    public class ScreeningsPageVm
    {
        public string CinemaName { get; set; } = "";
        public string City { get; set; } = "";
        public List<ScreeningsDayVm> Days { get; set; } = new();
        
    }
}
