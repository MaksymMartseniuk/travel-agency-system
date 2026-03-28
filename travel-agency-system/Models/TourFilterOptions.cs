using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public record TourFilterOptions
    {
        public FilterCategory Category { get; init; } = FilterCategory.All;
        public SortOrder Order { get; init; } = SortOrder.Ascending;
        public string? MinValue { get; init; }
        public string? MaxValue { get; init; }

    }
}
