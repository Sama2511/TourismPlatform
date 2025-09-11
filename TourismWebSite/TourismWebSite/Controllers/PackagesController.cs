using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PackagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var tours = db.Tours.ToList();

            var counts = db.Bookings
                           .GroupBy(b => b.TourId)
                           .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.BookingCounts = counts;

            ViewBag.TotalBookings = counts.Values.Sum();

            var endedTourIds = tours.Where(t => t.EndDate <= DateTime.Today).Select(t => t.Id).ToList();
            ViewBag.TotalRevenue = db.Bookings
                                     .Where(b => endedTourIds.Contains(b.TourId))
                                     .Select(b => b.Tour.Price)
                                     .DefaultIfEmpty(0)
                                     .Sum();

            return View(tours);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tours tours, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View(tours);

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var okExt = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
                if (!okExt.Contains(ext))
                {
                    ModelState.AddModelError("", "Only JPG/PNG/GIF images are allowed.");
                    return View(tours);
                }

                var folder = Server.MapPath("~/Content/Uploads/Tours");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(folder, fileName);
                imageFile.SaveAs(fullPath);

                tours.ImageUrl = $"/Content/Uploads/Tours/{fileName}";
            }

            db.Tours.Add(tours);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tours = db.Tours.Find(id);
            if (tours == null) return HttpNotFound();

            return View(tours);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tours tours = db.Tours.Find(id);
            if (tours == null)
            {
                return HttpNotFound();
            }
            return View(tours);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tours tours = db.Tours.Find(id);
            db.Tours.Remove(tours);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       // protected override void Dispose(bool disposing)
       // {
            //if (disposing)
          //  {
              //  db.Dispose();
           // }
           // base.Dispose(disposing);
           // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tours tour, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View(tour);

            var existing = db.Tours.Find(tour.Id);
            if (existing == null) return HttpNotFound();

            existing.Name = tour.Name;
            existing.Description = tour.Description;
            existing.Price = tour.Price;
            existing.DurationDays = tour.DurationDays;
            existing.Destination = tour.Destination;
            existing.StartDate = tour.StartDate;
            existing.EndDate = tour.EndDate;

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                var okExt = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();
                if (!okExt.Contains(ext))
                {
                    ModelState.AddModelError("", "Only JPG/PNG/GIF images are allowed.");
                    return View(existing);
                }

                var folder = Server.MapPath("~/Content/Uploads/Tours");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(folder, fileName);
                imageFile.SaveAs(fullPath);

                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    var oldPath = Server.MapPath(existing.ImageUrl);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                existing.ImageUrl = $"/Content/Uploads/Tours/{fileName}";
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
