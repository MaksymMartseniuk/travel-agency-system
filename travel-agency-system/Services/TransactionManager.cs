using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TransactionManager
    {
        private readonly StorageService _storage = StorageService.GetInstance;

        public async Task RecordTransactionAsync(Guid customerId, TravelPackage tour)
        {
            var newTransaction = new PaymentTransaction(customerId);
            newTransaction.AddTour(tour);
            string fileName = _storage.GetFileName<PaymentTransaction>();
            var allTransactions = await _storage.LoadFromFileAsync<PaymentTransaction>(fileName);
            allTransactions.Add(newTransaction);
            await _storage.SaveToFileAsync(fileName, allTransactions);
        }
        public async Task<List<PaymentTransaction>> GetAllTransactionsAsync()
        {
            string fileName = _storage.GetFileName<PaymentTransaction>();
            return await _storage.LoadFromFileAsync<PaymentTransaction>(fileName);
        }

        public async Task<List<PaymentTransaction>> GetTransactionsByCustomerAsync(Guid customerId)
        {
            var allTransactions = await GetAllTransactionsAsync();
            return allTransactions.Where(t => t.PayerId == customerId).ToList();
        }
    }
}
