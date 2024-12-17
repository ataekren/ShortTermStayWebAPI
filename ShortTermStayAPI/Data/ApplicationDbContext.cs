using Microsoft.EntityFrameworkCore;
using ShortTermStayAPI.Model.Entities;
using System.Reflection;

namespace ShortTermStayAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listing>()
                .Property(l => l.PricePerNight)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
