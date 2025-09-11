using Microsoft.AspNet.Identity;          // GetUserId()
using System;
using System.Collections.Generic;         // HashSet<>, IDictionary<>
using System.Data.Entity;                 // Include()
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // ---------- CREATE (GET) ----------
        [Authorize]
        public ActionResult Create(int bookingId)
        {
            var userId = User.Identity.GetUserId();

            // Must be this user's booking
            var booking = db.Bookings
                            .Include(b => b.Tour)
                            .FirstOrDefault(b => b.BookingId == bookingId && b.UserId == userId);

            if (booking == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Block duplicate review
            if (db.Reviews.Any(r => r.BookingId == bookingId))
            {
                TempData["Info"] = "You already reviewed this booking.";
                return RedirectToAction("Index", "Bookings");
            }

            // Optional: only after tour ends
            if (booking.Tour != null && booking.Tour.EndDate > DateTime.Today)
            {
                TempData["Info"] = "You can review this tour after it ends.";
                return RedirectToAction("Index", "Bookings");
            }

            ViewBag.BookingId = booking.BookingId;
            ViewBag.TourName = booking.Tour?.Name;
            return View();
        }

        // ---------- CREATE (POST) ----------
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review)
        {
            var userId = User.Identity.GetUserId();

            // Server-side: booking must belong to user
            var booking = db.Bookings
                            .Include(b => b.Tour)
                            .FirstOrDefault(b => b.BookingId == review.BookingId && b.UserId == userId);

            if (booking == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Block duplicate
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

        // ---------- INDEX (PUBLIC) ----------
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

            // distribution 1..5
            var distribution = Enumerable.Range(1, 5)
                                         .ToDictionary(star => star,
                                                       star => reviews.Count(r => r.Rating == star));

            // mark "verified" when review.UserId == booking.UserId
            var verifiedIds = new HashSet<int>(
                reviews.Where(r => r.Booking != null && r.UserId == r.Booking.UserId)
                       .Select(r => r.Id)
            );

            ViewBag.TotalReviews = total;
            ViewBag.AverageRating = avg;
            ViewBag.Distribution = distribution; // IDictionary<int,int>
            ViewBag.VerifiedIds = verifiedIds;  // HashSet<int>

            return View(reviews);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
