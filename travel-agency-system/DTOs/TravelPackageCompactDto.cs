using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.DTOs
{
    public class TravelPackageCompactDto
    {
        public Guid Id { get; set; }
        public string? TourName { get; set; }
        public string Price { get; set; } = string.Empty;
        public int Days { get; set; }
    }
}
