using System;
using System.ComponentModel.DataAnnotations;

namespace TourismWebSite.Models
{
    public class Payment
    {
        [Required]
        public int TourId { get; set; }

        public string TourName { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }

        [Required, Display(Name = "Cardholder Name")]
        public string CardName { get; set; }

        [Required, Display(Name = "Card Number")]
        [RegularExpression(@"\d{16}", ErrorMessage = "Enter 16 digits.")]
        public string CardNumber { get; set; }

        [Required, Display(Name = "Expiry (MM/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Use MM/YY.")]
        public string Expiry { get; set; }

        [Required, Display(Name = "CVV")]
        [RegularExpression(@"\d{3,4}", ErrorMessage = "3–4 digits.")]
        public string CVV { get; set; }
    }
}
