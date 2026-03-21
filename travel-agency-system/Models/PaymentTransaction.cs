using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class PaymentTransaction: Entity
    {
        public Guid PayerId { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }

        public List<TravelPackage> PurchasedTours { get; set; }

        public PaymentTransaction()
        {
            TransactionDate = DateTime.Now;
            Amount = 0.0;
            PayerId = Guid.NewGuid();
            PurchasedTours = new List<TravelPackage>();
        }
        public PaymentTransaction(Guid id, Guid payerId, double amount, DateTime transactionDate)
            : base(id)
        {
            this.PayerId = payerId;
            this.Amount = amount;
            this.TransactionDate = transactionDate;
            PurchasedTours = new List<TravelPackage>();
        }

        public new bool IsValid()
        {
            return base.IsValid()&&
                   PayerId != Guid.Empty &&
                   Amount > 0 &&
                   PurchasedTours != null;

        }
        public sealed override string GetInfo()
        {
            return $"[ТРАНЗАКЦІЯ] Сума: {Amount}$ | Турів: {PurchasedTours.Count} | Дата: {TransactionDate.ToShortDateString()}";
        }
    }
}
