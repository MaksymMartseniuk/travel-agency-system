using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using travel_agency_system.Models;
using Microsoft.EntityFrameworkCore;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TransactionManager
    {
        public async Task RecordTransactionAsync(Guid customerId, TravelPackage tour)
        {
            using (var context = new AppDbContext())
            {
                var newTransaction = new PaymentTransaction(customerId);
                
                var trackedTour = await context.TravelPackages.FindAsync(tour.Id);
                if (trackedTour != null)
                {
                    newTransaction.AddTour(trackedTour);
                }
                else
                {
                    newTransaction.AddTour(tour);
                }

                context.PaymentTransactions.Add(newTransaction);
                await context.SaveChangesAsync();
            }
        }
        public async Task<List<PaymentTransaction>> GetAllTransactionsAsync()
        {
            using (var context = new AppDbContext())
            {
                return await context.PaymentTransactions
                    .Include(t => t.PurchasedTours)
                    .ToListAsync();
            }
        }

        public async Task<List<PaymentTransaction>> GetTransactionsByCustomerAsync(Guid customerId)
        {
            using (var context = new AppDbContext())
            {
                return await context.PaymentTransactions
                    .Include(t => t.PurchasedTours)
                    .Where(t => t.PayerId == customerId)
                    .ToListAsync();
            }
        }
    }
}
