using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TourismWebSite.Models;

namespace TourismWebSite.Controllers
{
    public class PackagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Packages
        public ActionResult Index()
        {
            return View(db.Tours.ToList());
        }

        // GET: Packages/Details/5
        public ActionResult Details(int? id)
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

        // GET: Packages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Packages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,DurationDays,Destination,StartDate,EndDate")] Tours tours)
        {
            if (ModelState.IsValid)
            {
                db.Tours.Add(tours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tours);
        }

        // GET: Packages/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Packages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,DurationDays,Destination,StartDate,EndDate")] Tours tours)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tours).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tours);
        }

        // GET: Packages/Delete/5
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
