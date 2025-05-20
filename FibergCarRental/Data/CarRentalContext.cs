using FibergCarRental.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FibergCarRental.Data
{
    public class CarRentalContext :DbContext
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
