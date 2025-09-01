using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TourismWebSite.Models
{
    public class Booking
    {
        public int BookingId { get; set; }                    
        [Required] public string UserId { get; set; }  
        [Required] public int TourId { get; set; }    


        public virtual ApplicationUser User { get; set; }
        public virtual Tours Tour { get; set; }
    }
}