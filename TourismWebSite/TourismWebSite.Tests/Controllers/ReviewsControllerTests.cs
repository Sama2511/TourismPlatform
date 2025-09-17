using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Web.Mvc;
using TourismWebSite.Controllers;
using TourismWebSite.Models;
using TourismWebSite.Tests.Infrastructure;

namespace TourismWebSite.Tests.Controllers
{
    [TestClass]
    public class ReviewsControllerTests
    {
        [TestMethod]
        public void Create_Post_SavesReview_And_Redirects()
        {
            var db = new TestApplicationDbContext();
            var tour = new Tours { Id = 3, Name = "Harbour", EndDate = DateTime.Today.AddDays(-1) };
            ((InMemoryDbSet<Tours>)db.Tours).Add(tour);

            var booking = new Booking { BookingId = 77, UserId = "U3", TourId = 3, Tour = tour };
            ((InMemoryDbSet<Booking>)db.Bookings).Add(booking);

            var controller = new ReviewsController(db);
            controller.SetUser("U3");

            var review = new Review { BookingId = 77, Rating = 5, Comment = "Awesome!" };

            var result = controller.Create(review) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, db.Reviews.Count());
            Assert.AreEqual("U3", db.Reviews.First().UserId);
            Assert.AreEqual("Bookings", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}



