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

        // GET: Packages
        public ActionResult Index()
        {
            return View(db.Tours.ToList());
        }

        // GET: Packages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tours tours, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View(tours);

            // handle upload
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

        // GET: Packages/Edit/5
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

        // POST: Packages/Delete/5
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

        // POST: Packages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tours tour, HttpPostedFileBase imageFile)
        {
            if (!ModelState.IsValid) return View(tour);

            var existing = db.Tours.Find(tour.Id);
            if (existing == null) return HttpNotFound();

            // update scalar fields
            existing.Name = tour.Name;
            existing.Description = tour.Description;
            existing.Price = tour.Price;
            existing.DurationDays = tour.DurationDays;
            existing.Destination = tour.Destination;
            existing.StartDate = tour.StartDate;
            existing.EndDate = tour.EndDate;

            // optional: replace image
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

                // delete old image if present
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

        // (rest of your controller unchanged: Details/Delete/Dispose...)
    }
}
