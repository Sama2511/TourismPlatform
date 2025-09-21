using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Mvc;
using TourismWebSite.Controllers;
using TourismWebSite.Models;
using TourismWebSite.Tests.Infrastructure;

namespace TourismWebSite.Tests.Controllers
{
    [TestClass]
    public class BookingsControllerTests
    {
        [TestMethod]
        public void AddBooking()
        {
            var db = new TestApplicationDbContext();
            ((InMemoryDbSet<Tours>)db.Tours).Add(new Tours { Id = 1, Name = "Blue Mountains" });

            var controller = new BookingsController(db);
            controller.SetUser("U1"); 

            var result = controller.Create(1) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, db.Bookings.Count());
            Assert.AreEqual("U1", db.Bookings.First().UserId);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}





