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
    public class ToursControllerTests
    {
        [TestMethod]
        public void MakePayment()
        {
            var db = new TestApplicationDbContext();
            ((InMemoryDbSet<Tours>)db.Tours).Add(new Tours
            {
                Id = 5,
                Name = "Outback",
                Destination = "Alice Springs",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                Price = 500m
            });

            var controller = new ToursController(db);
            controller.SetUser("U2");

            var vm = new Payment
            {
                TourId = 5,
                CardName = "Test User",
                CardNumber = "1234567812345678",
                Expiry = "12/30",
                CVV = "123"
            };

            var result = controller.Payment(vm) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, db.Bookings.Count());
            Assert.AreEqual("U2", db.Bookings.First().UserId);
            Assert.AreEqual("Bookings", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}




