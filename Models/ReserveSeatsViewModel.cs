using System.Collections.Generic;
using CineMate.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineMate.Models
{
    public class ReserveSeatsViewModel
    {
        // the reservation stub we created in step1
        public Reservation Reservation { get; set; }

        // the linked screening (with Movie + Cinema included)
        public Screening Screening { get; set; }
        public int ScreeningId { get; set; }


        // the exact seats the user picked in step1
        public List<Seat> Seats { get; set; }

        // re-posted seat‐ids
        public int[] SelectedSeatIds { get; set; }

        // category they choose in the dropdown
        public string SelectedCategory { get; set; }

        // dropdown options
        public List<SelectListItem> Categories { get; set; }
    }
}
