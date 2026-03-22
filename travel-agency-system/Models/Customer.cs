using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class Customer:User
    {
        public double Balance { get; set; }

        public Customer()
        {
            this.Balance = 0.0;
        }

        public Customer(string? email, string? passwordHash,double balance):base(email,passwordHash)
        {
            this.Balance = balance;
        }
        public new bool IsValid()
        {
            return base.IsValid() && this.Balance >= 0.0;
        }

        public override string GetRole() => "Customer";
        public override string GetInfo() => base.GetInfo() + $" | Balance: {Balance}$";
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
