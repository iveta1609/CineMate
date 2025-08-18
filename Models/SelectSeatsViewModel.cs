using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using CineMate.Data.Entities;

namespace CineMate.Models
{
    public class SelectSeatsViewModel
    {
        public int ScreeningId { get; set; }
        public Screening? Screening { get; set; }   

        public List<Seat> Seats { get; set; } = new();

        public int[] SelectedSeatIds { get; set; } = Array.Empty<int>();
        public string? SelectedCategory { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
