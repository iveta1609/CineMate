using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CineMate.Data.Entities
{
    public class Screening
    {
        public int Id { get; set; }
        [Required]
        public int CinemaId { get; set; }
        [ValidateNever]
        public Cinema Cinema { get; set; } = null!;
        [Required]
        public int MovieId { get; set; }
        [ValidateNever]
        public Movie Movie { get; set; } = null!;
        public string? Format { get; set; }  
        public string? Audio { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
