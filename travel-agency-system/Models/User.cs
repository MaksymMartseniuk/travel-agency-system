using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class User: Entity
    {
        public string? Email { get; set; }
        
        public string? PasswordHash { get; set; }

        public User()
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
        }

        public User(string? email, string? passwordHash):base()
        {
            Email = email;
            PasswordHash = passwordHash;
        }

        public new bool IsValid()
        {
            return base.IsValid() &&
                !string.IsNullOrEmpty(Email)&&
                !string.IsNullOrEmpty(PasswordHash);
        }
    }
}
