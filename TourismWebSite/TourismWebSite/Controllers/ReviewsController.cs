using Microsoft.AspNet.Identity;          
using System;
using System.Collections.Generic;      
using System.Data.Entity;                
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        public ReviewsController() : this(new ApplicationDbContext()) { }
        public ReviewsController(ApplicationDbContext context) { db = context; }

        [Authorize]
        public ActionResult Create(int bookingId)
        {
            var userId = User.Identity.GetUserId();

            var booking = db.Bookings
                            .Include(b => b.Tour)
                            .FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (db.Reviews.Any(r => r.BookingId == bookingId))
            {
                TempData["Info"] = "You already reviewed this booking.";
                return RedirectToAction("Index", "Bookings");
            }

            if (booking.Tour != null && booking.Tour.EndDate > DateTime.Today)
            {
                TempData["Info"] = "You can review this tour after it ends.";
                return RedirectToAction("Index", "Bookings");
            }

            ViewBag.BookingId = booking.BookingId;
            ViewBag.TourName = booking.Tour?.Name;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review)
        {
            var userId = User.Identity.GetUserId();

            var booking = db.Bookings
                            .Include(b => b.Tour)
                            .FirstOrDefault(b => b.BookingId == review.BookingId && b.UserId == userId);

            if (booking == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            if (db.Reviews.Any(r => r.BookingId == review.BookingId))
            {
                TempData["Info"] = "You already reviewed this booking.";
                return RedirectToAction("Index", "Bookings");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TourName = booking.Tour?.Name;
                ViewBag.BookingId = booking.BookingId;
                return View(review);
            }

            review.CreatedAt = DateTime.Now;
            review.UserId = userId;

            db.Reviews.Add(review);
            db.SaveChanges();

            TempData["Success"] = "Thanks for your review!";
            return RedirectToAction("Index", "Bookings");
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var reviews = db.Reviews
                            .Include(r => r.Booking.Tour)
                            .Include(r => r.User)
                            .OrderByDescending(r => r.CreatedAt)
                            .ToList();

            int total = reviews.Count;
            double avg = total > 0 ? reviews.Average(r => r.Rating) : 0.0;

            var distribution = Enumerable.Range(1, 5)
                                         .ToDictionary(star => star,
                                                       star => reviews.Count(r => r.Rating == star));

            var verifiedIds = new HashSet<int>(
                reviews.Where(r => r.Booking != null && r.UserId == r.Booking.UserId)
                       .Select(r => r.Id)
            );

            ViewBag.TotalReviews = total;
            ViewBag.AverageRating = avg;
            ViewBag.Distribution = distribution; 
            ViewBag.VerifiedIds = verifiedIds;  

            return View(reviews);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
