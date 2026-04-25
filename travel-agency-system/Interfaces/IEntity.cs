using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Interfaces
{
    public interface IEntity
    {
        public Guid Id { get; set; }

        public bool IsValid();
    }
}
