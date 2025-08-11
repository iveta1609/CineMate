using System.Collections.Generic;

namespace CineMate.Data.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();
    }
}
