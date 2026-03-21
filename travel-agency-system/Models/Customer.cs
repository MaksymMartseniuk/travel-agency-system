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

        public Customer(Guid id, string? email, string? passwordHash,double balance):base(id,email,passwordHash)
        {
            this.Balance = balance;
        }
        public new bool IsValid()
        {
            return base.IsValid() && this.Balance >= 0.0;
        }
    }
}
