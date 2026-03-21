using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace travel_agency_system.Models
{
    public class Admin:User
    {
        public bool CanEditCatalog { get; set; }

        public Admin()
        {
            CanEditCatalog = true;
        }

        public Admin (Guid id, string? email, string? passwordHash,bool canEditCatalog):base(id, email, passwordHash){
            this.CanEditCatalog=canEditCatalog;
        }
    }
}
