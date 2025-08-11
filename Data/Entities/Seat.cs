using CineMate.Data.Entities.CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CineMate.Data.Entities
{
    public class Seat
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Screening))]
        public int ScreeningId { get; set; }
        [ValidateNever]
        public Screening Screening { get; set; } = null!;

        [Required]
        [Range(1, 7)]
        public int Row { get; set; }

        [Required]
        [Range(1, 10)]
        public int Number { get; set; }

        public bool IsAvailable { get; set; } = true;

        public string Category { get; set; } = "Adult";

        public ICollection<ReservationSeat> ReservationSeats { get; set; } = new List<ReservationSeat>();
    }
}
