using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Interfaces
{
    public interface ISearchable
    {
        bool Matches(string searchQuery);
    }
}
