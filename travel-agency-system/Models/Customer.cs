using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace travel_agency_system.Models
{
    public class Customer:User
    {
        [JsonInclude]
        [JsonPropertyName("Balance")]
        public double Balance { get; private set; }

        [JsonConstructor]
        public Customer()
        {
            this.Balance = 0.0;
        }

        public Customer(string? email, string? passwordHash,double balance):base(email,passwordHash)
        {
            this.Balance = balance;
        }
        public override bool IsValid()
        {
            return base.IsValid() && this.Balance >= 0.0;
        }
        public void TopUp(double amount) { if (amount > 0) Balance += amount; }
        public bool CanAfford(double price) => Balance >= price;

        public double MakePurchase(double price)
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
