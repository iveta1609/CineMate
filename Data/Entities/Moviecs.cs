using System.Collections.Generic;

namespace CineMate.Data.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Director { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public string? PosterUrl { get; set; }   
        public string Genre { get; set; } = null!;
        public string Synopsis { get; set; } = null!;
        public int DurationMinutes { get; set; }
        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
