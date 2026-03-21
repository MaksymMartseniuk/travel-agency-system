using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace travel_agency_system.Models
{
    public sealed class Admin:User
    {
        public bool CanEditCatalog { get; set; }

        public Admin()
        {
            CanEditCatalog = true;
        }

        public Admin (string? email, string? passwordHash,bool canEditCatalog):base(email, passwordHash){
            this.CanEditCatalog=canEditCatalog;
        }
    }
}
