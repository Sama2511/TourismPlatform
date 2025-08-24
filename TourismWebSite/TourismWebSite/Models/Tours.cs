using System;
using System.Collections.Generic;
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
        public int DurationDays { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}