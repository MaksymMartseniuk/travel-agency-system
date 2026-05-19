using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace travel_agency_system.Models
{
    [Table("Transactions")]
    public class PaymentTransaction: Entity
    {
        [Required]
        public Guid PayerId { get; set; }

        [ForeignKey(nameof(PayerId))]
        public virtual Customer? Payer { get; set; }
        private decimal _amount;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount
        {
            get => _amount;
            private set => _amount = value;
        }
        [Required]
        public DateTime TransactionDate { get; set; }

        public virtual ICollection<TravelPackage> PurchasedTours { get; set; } = new List<TravelPackage>();

        public PaymentTransaction()
        {
            TransactionDate = DateTime.Now;
            Amount = 0.0m;
            PayerId = Guid.Empty;
        }
        public PaymentTransaction(Guid payerId) : base()
        {
            this.PayerId = payerId;
            this.Amount = 0.0m;
            this.TransactionDate = DateTime.Now;
        }

        public void AddTour(TravelPackage tour)
        {
            if (tour != null)
            {
                PurchasedTours.Add(tour);
                Amount += (decimal)tour.Price;
            }
        }

        public override bool IsValid()
        {
            return base.IsValid() &&
                   PayerId != Guid.Empty &&
                   Amount > 0 &&
                   PurchasedTours != null &&
                   PurchasedTours.Count > 0;
        }
    }
}
