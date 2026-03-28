using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using travel_agency_system.Interfaces;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TourFilterService : ITourFilterService
    {
        public event Action<IEnumerable<TravelPackage>>? OnFilterCompleted;

        public void ApplyFilters(IEnumerable<TravelPackage> tours, TourFilterOptions options)
        {
            if (tours == null) return;

            var filteredTours = ApplyRangeFilters(tours, options);
            var sortedTours = ApplySorting(filteredTours, options);

            OnFilterCompleted?.Invoke(sortedTours);
        }

        private IEnumerable<TravelPackage> ApplyRangeFilters(IEnumerable<TravelPackage> tours, TourFilterOptions options)
        {
            var result = tours;

            if (options.Category == FilterCategory.Price)
            {
                if (double.TryParse(options.MinValue, out double minPrice))
                    result = result.Where(t => t.Price >= minPrice);

                if (double.TryParse(options.MaxValue, out double maxPrice))
                    result = result.Where(t => t.Price <= maxPrice);
            }
            if (DateTime.TryParse(options.MinValue, out DateTime minDate))
            {
                result = result.Where(t => t.StartDate.Date >= minDate.Date);
            }
            if (DateTime.TryParse(options.MaxValue, out DateTime maxDate))
            {
                result = result.Where(t => t.StartDate.Add(t.Duration).Date <= maxDate.Date);
            }

            return result;
        }

        private IEnumerable<TravelPackage> ApplySorting(IEnumerable<TravelPackage> tours, TourFilterOptions options)
        {
            return (options.Category, options.Order) switch
            {
                (FilterCategory.Price, SortOrder.Ascending) => tours.OrderBy(t => t.Price),
                (FilterCategory.Price, SortOrder.Descending) => tours.OrderByDescending(t => t.Price),
                (FilterCategory.Date, SortOrder.Ascending) => tours.OrderBy(t => t.StartDate),
                (FilterCategory.Date, SortOrder.Descending) => tours.OrderByDescending(t => t.StartDate),
                _ => tours
            };
        }
    }
}
