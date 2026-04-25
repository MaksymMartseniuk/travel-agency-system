using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Interfaces
{
    public interface ISortable<T, TOptions>
    {
        IEnumerable<T> ApplySort(IEnumerable<T> items, TOptions options);
    }
}
