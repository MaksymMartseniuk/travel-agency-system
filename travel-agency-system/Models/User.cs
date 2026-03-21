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

        public User(Guid id,string? email, string? passwordHash):base(id)
        {
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
