using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public HomeController() : this(new ApplicationDbContext()) { }
        public HomeController(ApplicationDbContext context) { db = context; }
        public ActionResult Index()
        {
            var featured = db.Tours
                             .OrderByDescending(t => t.StartDate)
                             .Take(3)
                             .ToList();

            ViewBag.Destinations = db.Tours.Select(t => t.Destination).Distinct().Count();
            ViewBag.HappyTravelers = 128;      
            ViewBag.AvgRating = 4.6;           
            ViewBag.YearsExperience = 3;      

            return View(featured);           
        }


    }
}