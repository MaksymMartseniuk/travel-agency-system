using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.DTOs;
using travel_agency_system.Models;

namespace travel_agency_system.Extensions
{
    public static class TravelPackageExtensions
    {
        public static IEnumerable<TravelPackageCompactDto> ToCompactView(this IEnumerable<TravelPackage> tours)
        {
            return tours.Select(t => new TravelPackageCompactDto
            {
                Id = t.Id,
                TourName = t.Name,
                Price = $"{t.Price:F2} $",
                Days = t.Duration.Days
            });
        }

        public static IEnumerable<TravelPackageFullDto> ToFullView(this IEnumerable<TravelPackage> tours)
        {
            return tours.Select(t => new TravelPackageFullDto
            {
                Id = t.Id,
                Name = t.Name,
                Price = $"{t.Price:F2} $",
                Description = t.Description,
                StartDate = t.StartDate.ToString("dd.MM.yyyy"),
                Duration = $"{t.Duration.Days} days",
                Activities = string.Join(", ", t.Activities)
            });
        }
        public static TourStatisticsDto GetStatistics(this IEnumerable<TravelPackage> tours)
        {
            if (tours == null || !tours.Any())
            {
                return new TourStatisticsDto { TotalCount = 0, AveragePrice = "0.00 $" };
            }

            return new TourStatisticsDto
            {
                TotalCount = tours.Count(),
                AveragePrice = $"{tours.Average(t => t.Price):F2} $"
            };
        }
    }
}
