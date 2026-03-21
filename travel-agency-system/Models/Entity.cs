using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        public Entity ()
        {
            this.Id = Guid.NewGuid ();
        }

        public Entity(Guid id)
        {
            this.Id = id;
        }
    }
}
