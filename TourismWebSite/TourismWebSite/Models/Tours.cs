using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

 namespace TourismWebSite.Models
{
    public class Tours
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        public int DurationDays { get; set; }
        public string Destination { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public string ImageUrl { get; set; }
    }
}