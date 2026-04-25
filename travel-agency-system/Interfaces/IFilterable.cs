using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Interfaces
{
    public interface IFilterable<T>
    {
        bool IsMatch(T options);
    }
}
