using System.Collections.Generic;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineMate.Models
{
    public class SelectSeatsViewModel
    {
        public Screening Screening { get; set; } = default!;

        public List<Seat> Seats { get; set; } = new();

        public int[] SelectedSeatIds { get; set; } = new int[0];

        public string? SelectedCategory { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
