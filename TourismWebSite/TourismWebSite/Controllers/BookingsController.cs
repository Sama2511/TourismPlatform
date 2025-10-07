using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;                
using System.Collections.Generic;        
using Microsoft.AspNet.Identity;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{

    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        public BookingsController() : this(new ApplicationDbContext()) { }

        public BookingsController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id)
        {
            var userId = User.Identity.GetUserId();

            var tour = db.Tours.Find(id);
            if (tour == null)
            {
                TempData["Error"] = "That tour no longer exists.";
                return RedirectToAction("Index", "Tours");
            }

         

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
        public ActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();

            var booking = db.Bookings
                            .Include(b => b.Tour)
                            .FirstOrDefault(b => b.BookingId == id && b.UserId == userId);

            if (booking == null) return HttpNotFound();

            if (booking.Tour != null && booking.Tour.EndDate <= DateTime.Today)
            {
                TempData["Error"] = "You can’t cancel a booking after the tour has finished.";
                return RedirectToAction("Index");
            }

            db.Bookings.Remove(booking);
            db.SaveChanges();
            TempData["Message"] = "Booking cancelled.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
