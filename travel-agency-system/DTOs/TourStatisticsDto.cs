using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.DTOs
{
    public class TourStatisticsDto
    {
        public int TotalCount { get; set; }
        public string AveragePrice { get; set; } = "0.00 $";
    }
}
