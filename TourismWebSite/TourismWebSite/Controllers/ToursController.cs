using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class ToursController : Controller
    {
        // GET: Tours
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var tours = db.Tours.ToList(); 

            return View(tours);
        }
        public ActionResult Details(int id)
        {
            var tour = db.Tours.Find(id);
            if (tour == null) return HttpNotFound();
            return View(tour); 
        }
    }
}
