using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using travel_agency_system.Services;

namespace travel_agency_system.Models
{
    public class PaymentTransaction: Entity
    {
        public Guid PayerId { get; set; }
        public double Amount { get; private set; }
        public DateTime TransactionDate { get; set; }

        [JsonInclude]
        [JsonPropertyName("PurchasedTours")]
        private List<TravelPackage> _purchasedTours;

        [JsonIgnore]
        public IReadOnlyList<TravelPackage> PurchasedTours => _purchasedTours.AsReadOnly();

        public PaymentTransaction()
        {
            TransactionDate = DateTime.Now;
            Amount = 0.0;
            PayerId = Guid.Empty;
            _purchasedTours = new List<TravelPackage>();
        }
        public PaymentTransaction(Guid payerId)
            : base()
        {
            this.PayerId = payerId;
            this.Amount = 0.0;
            this.TransactionDate = DateTime.Now;
            _purchasedTours = new List<TravelPackage>();
        }

        public void AddTour(TravelPackage tour)
        {
            if (tour != null)
            {
                _purchasedTours.Add(tour);
                Amount += tour.Price;
            }
        }

        public new bool IsValid()
        {
            return base.IsValid()&&
                   PayerId != Guid.Empty &&
                   Amount > 0 &&
                   _purchasedTours != null &&
                   _purchasedTours.Count > 0;

        }
        public sealed override string GetInfo()
        {
            return $"[ТРАНЗАКЦІЯ] Сума: {Amount}$ | Турів: {PurchasedTours.Count} | Дата: {TransactionDate.ToShortDateString()}";
        }
    }
}
