using System.Collections.Generic;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineMate.Models
{
    public class ReserveSeatsViewModel
    {
        public Reservation Reservation { get; set; }

        public Screening Screening { get; set; }
        public int ScreeningId { get; set; }


        public List<Seat> Seats { get; set; }

        public int[] SelectedSeatIds { get; set; }

        public string SelectedCategory { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
