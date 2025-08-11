using System.ComponentModel.DataAnnotations;

namespace CineMate.Data.Entities
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a city.")]
        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public ICollection<Screening> Screenings { get; set; } = new List<Screening>();
    }
}
