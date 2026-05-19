using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.DTOs
{
    public class TravelPackageFullDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string Price { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Activities { get; set; } = string.Empty;
    }
}
