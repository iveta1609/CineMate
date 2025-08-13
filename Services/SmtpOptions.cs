namespace CineMate.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string? User { get; set; }
        public string? Pass { get; set; }
        public string From { get; set; } = "noreply@cinemate.local";
        public string FromName { get; set; } = "CineMate";
        public bool UseSsl { get; set; } = false; 
    }
}
