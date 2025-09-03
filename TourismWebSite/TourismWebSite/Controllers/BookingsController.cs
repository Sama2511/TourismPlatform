using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;                 // Include
using System.Collections.Generic;         // HashSet
using Microsoft.AspNet.Identity;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    [Authorize] // must be logged in to book/see bookings
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // POST: /Bookings/Create/5  (5 = TourId)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id) // id = TourId
        {
            var userId = User.Identity.GetUserId();

            // Ensure tour exists (and optionally is bookable)
            var tour = db.Tours.Find(id);
            if (tour == null)
            {
                TempData["Error"] = "That tour no longer exists.";
                return RedirectToAction("Index", "Tours");
            }

            // Optional: block booking past-end-date
            // if (tour.EndDate < DateTime.Today)
            // {
            //     TempData["Error"] = "This tour is no longer available.";
            //     return RedirectToAction("Index", "Tours");
            // }

            // Prevent duplicate booking of same tour by same user
            bool already = db.Bookings.Any(b => b.UserId == userId && b.TourId == id);
            if (!already)
            {
                db.Bookings.Add(new Booking { UserId = userId, TourId = id });
                db.SaveChanges();
                TempData["Success"] = "Tour booked!";
            }
            else
            {
                TempData["Info"] = "You’ve already booked this tour.";
            }

            return RedirectToAction("Index");
        }

        // GET: /Bookings
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var bookings = db.Bookings
                             .Include(b => b.Tour)
                             .Where(b => b.UserId == userId)
                             .ToList();

            var bookingIds = bookings.Select(b => b.BookingId).ToList();

            var reviewedIds = new HashSet<int>(
                db.Reviews
                  .Where(r => bookingIds.Contains(r.BookingId))
                  .Select(r => r.BookingId)
                  .ToList()
            );

            ViewBag.ReviewedIds = reviewedIds;
            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id) // id = BookingId (PK)
        {
            var userId = User.Identity.GetUserId();
            var booking = db.Bookings.Include(b => b.Tour).FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Index");
            }

            // Security: only the owner can delete their booking
            if (booking.UserId != userId)
            {
                return new HttpStatusCodeResult(403);
            }

            db.Bookings.Remove(booking);
            db.SaveChanges();
            TempData["Success"] = "Booking removed.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
