using Microsoft.AspNet.Identity;   // <-- needed for GetUserId()
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    // [Authorize] // only logged-in users can review
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews/Create
        // GET: Reviews/Create
        public ActionResult Create(int bookingId)
        {
            // Load the booking and tour name for display
            var booking = db.Bookings.Include(b => b.Tour).FirstOrDefault(b => b.BookingId == bookingId);
            if (booking == null) return HttpNotFound();

            ViewBag.BookingId = booking.BookingId;
            ViewBag.TourName = booking.Tour?.Name;

            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                review.UserId = User.Identity.GetUserId();
                db.Reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index", "Bookings");
            }

            // Reload tour name if validation fails
            var booking = db.Bookings.Include(b => b.Tour).FirstOrDefault(b => b.BookingId == review.BookingId);
            ViewBag.TourName = booking?.Tour?.Name;
            return View(review);
        }


        // Reviews Index (optional – e.g. admin-only)
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var reviews = db.Reviews.Include(r => r.Booking.Tour).ToList();
            return View(reviews);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
