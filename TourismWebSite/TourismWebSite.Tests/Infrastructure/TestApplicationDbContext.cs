using System.Data.Entity;
using TourismWebSite.Models;

namespace TourismWebSite.Tests.Infrastructure
{
    public class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext()
        {
            this.Tours = new InMemoryDbSet<Tours>();
            this.Bookings = new InMemoryDbSet<Booking>();
            this.Reviews = new InMemoryDbSet<Review>();
        }

        public override int SaveChanges() => 0;
        protected override void Dispose(bool disposing) { /* no-op */ }
    }
}
