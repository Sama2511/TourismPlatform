using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;  
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class ToursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(string q = null)
        {
            var tours = db.Tours.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                tours = tours.Where(t =>
                    (t.Name ?? "").ToLower().Contains(term) ||
                    (t.Destination ?? "").ToLower().Contains(term));
            }

            return View(tours.OrderBy(t => t.StartDate).ToList());
        }

        public ActionResult Details(int id)
        {
            var tour = db.Tours.Find(id);
            if (tour == null) return HttpNotFound();

            var reviews = db.Reviews
                            .Include(r => r.Booking)
                            .Include(r => r.User)       
                            .Where(r => r.Booking.TourId == id)
                            .ToList();

            ViewBag.Reviews = reviews;
            ViewBag.AvgRating = reviews.Any() ? (double?)reviews.Average(r => r.Rating) : null;

            int? bookingIdToReview = null;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var myBooking = db.Bookings.FirstOrDefault(b => b.UserId == userId && b.TourId == id);

                if (myBooking != null
                    && tour.EndDate <= DateTime.Today
                    && !db.Reviews.Any(r => r.BookingId == myBooking.BookingId))
                {
                    bookingIdToReview = myBooking.BookingId;
                }
            }
            ViewBag.ReviewBookingId = bookingIdToReview;

            return View(tour);
        }
        
    }
}
