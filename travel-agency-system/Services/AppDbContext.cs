using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class AppDbContext:DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<TravelPackage> TravelPackages { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=TravelAgencyDB;User=root;Password=12345;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TravelPackage>()
                .Property(e => e.Activities)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(a => Enum.Parse<TourActivity>(a))
                          .ToList()
                );
        }
    }
}
