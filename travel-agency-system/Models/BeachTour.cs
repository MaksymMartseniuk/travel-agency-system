using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class BeachTour: TravelPackage
    {
        public string? BeachType { get; set; }

        public BeachTour()
        {
            this.BeachType= string.Empty;
        }
        public BeachTour(Guid id, string name, double price, string desc, TimeSpan dur, DateTime start, string beachType)
            : base(id, name, price, desc, dur, start)
        {
            this.BeachType = beachType;
        }
    }
}
