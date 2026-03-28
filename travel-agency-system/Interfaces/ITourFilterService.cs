using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;

namespace travel_agency_system.Interfaces
{
    public interface ITourFilterService
    {
        event Action<IEnumerable<TravelPackage>>? OnFilterCompleted;
        void ApplyFilters(IEnumerable<TravelPackage> tours,TourFilterOptions options);
    }
}
