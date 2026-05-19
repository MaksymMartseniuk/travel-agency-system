using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<TravelPackage> TravelPackages { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=localhost;Database=TravelAgencyDB;User=root;Password=12345;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Customer>("Customer")
                .HasValue<Admin>("Admin");

            modelBuilder.Entity<Customer>()
                .Property(c => c.Balance)
                .HasColumnType("decimal(18,2)")
                .HasField("_balance")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Transactions)
                .WithOne(t => t.Payer)
                .HasForeignKey(t => t.PayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admin>()
                .Property(a => a.CanEditCatalog)
                .IsRequired();

            modelBuilder.Entity<TravelPackage>()
                .Property(e => e.Activities)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(a => Enum.Parse<TourActivity>(a))
                          .ToList()
                )
                .HasColumnType("varchar(500)");

            modelBuilder.Entity<TravelPackage>()
                .Property(e => e.Duration)
                .HasConversion(
                    v => v.Ticks,
                    v => TimeSpan.FromTicks(v)
                )
                .HasColumnType("bigint");

            modelBuilder.Entity<PaymentTransaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .HasField("_amount")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            modelBuilder.Entity<PaymentTransaction>()
                .HasMany(t => t.PurchasedTours)
                .WithMany(p => p.PaymentTransactions)
                .UsingEntity<Dictionary<string, object>>(
                    "TransactionTours",
                    j => j.HasOne<TravelPackage>()
                          .WithMany()
                          .HasForeignKey("TravelPackageId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<PaymentTransaction>()
                          .WithMany()
                          .HasForeignKey("PaymentTransactionId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("PaymentTransactionId", "TravelPackageId");
                        j.ToTable("TransactionTours");
                    }
                );
        }
    }
}
