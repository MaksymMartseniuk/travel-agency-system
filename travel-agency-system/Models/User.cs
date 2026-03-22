using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace travel_agency_system.Models
{
    public class User: Entity
    {
        public string? Email { get; set; }
        
        public string? PasswordHash { get; set; }
        public DateTime RegDate { get; set; }
        [JsonIgnore]
        public static User? ListHead;
        [JsonIgnore]
        public User? Next { get; set; }

        public User()
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            RegDate = DateTime.Now;
            Next = ListHead;
            ListHead = this;
        }

        public User(string? email, string? passwordHash):base()
        {
            Email = email;
            PasswordHash = passwordHash;
            RegDate = DateTime.Now;
        }

        public new bool IsValid()
        {
            return base.IsValid() &&
                !string.IsNullOrEmpty(Email)&&
                !string.IsNullOrEmpty(PasswordHash);
        }
        public virtual string GetRole() => "Base User";
        public override string GetInfo() => $"User: {Email}";
    }
}
