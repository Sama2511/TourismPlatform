using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;   // <-- needed for Include
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class ToursController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tours
        public ActionResult Index()
        {
            var tours = db.Tours.ToList();
            return View(tours);
        }

        public ActionResult Details(int id)
        {
            var tour = db.Tours.Find(id);
            if (tour == null) return HttpNotFound();

            // Include both Booking and the linked User
            var reviews = db.Reviews
                            .Include(r => r.Booking)
                            .Include(r => r.User)        // <-- this ensures we can access r.User.FullName
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
