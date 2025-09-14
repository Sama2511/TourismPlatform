using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
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

        // =============== PAYMENT FLOW ===============

        // Show payment page for a specific tour
        [Authorize]
        [HttpGet]
        public ActionResult Payment(int id)
        {
            var tour = db.Tours.Find(id);
            if (tour == null) return HttpNotFound();

            // if already booked, just send to bookings
            var userId = User.Identity.GetUserId();
            bool already = db.Bookings.Any(b => b.UserId == userId && b.TourId == id);
            if (already)
            {
                TempData["Msg"] = "You already booked this tour.";
                return RedirectToAction("Index", "Bookings");
            }

            var vm = new Payment
            {
                TourId = tour.Id,
                TourName = tour.Name,
                Destination = tour.Destination,
                StartDate = tour.StartDate,
                EndDate = tour.EndDate,
                Price = tour.Price
            };
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(Payment vm)
        {
            var tour = db.Tours.Find(vm.TourId);
            if (tour == null) return HttpNotFound();

            vm.TourName = tour.Name;
            vm.Destination = tour.Destination;
            vm.StartDate = tour.StartDate;
            vm.EndDate = tour.EndDate;
            vm.Price = tour.Price;

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var userId = User.Identity.GetUserId();

            bool already = db.Bookings.Any(b => b.UserId == userId && b.TourId == vm.TourId);
            if (!already)
            {
                db.Bookings.Add(new Booking
                {
                    UserId = userId,
                    TourId = vm.TourId
                });
                db.SaveChanges();
            }

            TempData["Msg"] = "Payment completed and booking created. Enjoy your trip!";
            return RedirectToAction("Index", "Bookings");
        }
    }
}
