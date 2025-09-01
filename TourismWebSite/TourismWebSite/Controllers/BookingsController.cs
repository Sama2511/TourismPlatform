using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TourismWebSite.Models;
using System.Data.Entity;  // for Include

[Authorize] // must be logged in to book/see bookings
public class BookingsController : Controller
{
    private readonly ApplicationDbContext db = new ApplicationDbContext();

    // POST: /Bookings/Create/5  (5 = TourId)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(int id) // id = TourId
    {
        var userId = User.Identity.GetUserId();
        // prevent duplicate booking of same tour by same user (optional)
        var already = db.Bookings.Any(b => b.UserId == userId && b.TourId == id);
        if (!already)
        {
            db.Bookings.Add(new Booking { UserId = userId, TourId = id });
            db.SaveChanges();
        }
        return RedirectToAction("Index"); // go to "My bookings"
    }

    // GET: /Bookings
    public ActionResult Index()
    {
        var userId = User.Identity.GetUserId();
        var myBookings = db.Bookings
                           .Where(b => b.UserId == userId)
                           .Include(b => b.Tour)  // so we can show tour details
                           .ToList();
        return View(myBookings);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) db.Dispose();
        base.Dispose(disposing);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        var booking = db.Bookings.Find(id);
        if (booking != null)
        {
            db.Bookings.Remove(booking);
            db.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}
