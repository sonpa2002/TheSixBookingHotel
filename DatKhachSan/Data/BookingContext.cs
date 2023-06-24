using DatKhachSan.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace DatKhachSan.Data
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options)
        {
        }

        public BookingContext()
        {
        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<CustomerReport> CustomerReports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerReport>()
                .HasKey(c => c.ReportID);
        }
    }
}
