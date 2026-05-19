using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace travel_agency_system.Models
{
    public class Customer:User
    {
        private decimal _balance;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance
        {
            get => _balance;
            private set => _balance = value;
        }

        public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
        public Customer()
        {
            this.Balance = 0.0m;
        }

        public Customer(string? email, string? passwordHash, double balance) : base(email, passwordHash)
        {
            this.Balance = (decimal)balance;
        }
        public override bool IsValid()
        {
            return base.IsValid() && this.Balance >= 0.0m;
        }
        public void TopUp(double amount) { if (amount > 0) Balance += (decimal)amount; }
        public bool CanAfford(decimal price) => Balance >= price;

        public decimal MakePurchase(decimal price)
        {
            if (CanAfford(price))
            {
                Balance -= price;
                return Balance;
            }
            else
            {
                throw new InvalidOperationException("Недостатньо коштів для покупки.");
            }
        }
    }
}
