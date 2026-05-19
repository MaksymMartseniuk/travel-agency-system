using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using travel_agency_system.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travel_agency_system.Models
{
    [Table("Users")]
    public class User: Entity, ISearchable
    {
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MaxLength(255)]
        public string? PasswordHash { get; set; }
        [Required]
        public DateTime RegDate { get; set; }

        public User()
        {
            Email = string.Empty;
            PasswordHash = string.Empty;
            RegDate = DateTime.Now;
        }

        public User(string? email, string? passwordHash):base()
        {
            Email = email;
            PasswordHash = passwordHash;
            RegDate = DateTime.Now;
        }

        public override bool IsValid()
        {
            return base.IsValid() &&
                !string.IsNullOrEmpty(Email)&&
                !string.IsNullOrEmpty(PasswordHash);
        }

        public bool Matches(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return true;
            return this.Email!=null && this.Email.Contains(searchQuery,StringComparison.OrdinalIgnoreCase);
        }
    }
}
