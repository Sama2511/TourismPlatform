using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Mvc;
using TourismWebSite.Controllers;
using TourismWebSite.Models;
using TourismWebSite.Tests.Infrastructure;

namespace TourismWebSite.Tests.Controllers
{
    [TestClass]
    public class PackagesControllerTests
    {
        [TestMethod]
        public void DeleteConfirmed_RemovesTour_And_Redirects()
        {
            var db = new TestApplicationDbContext();
            ((InMemoryDbSet<Tours>)db.Tours).Add(new Tours { Id = 9, Name = "Past Tour", Price = 100m });

            var controller = new PackagesController(db);
            controller.SetUser("admin");

            var result = controller.DeleteConfirmed(9) as RedirectToRouteResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, db.Tours.Count());
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
