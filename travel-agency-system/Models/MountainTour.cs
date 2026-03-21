using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class MountainTour: TravelPackage
    {
        public bool HasGuide { get; set; }

        public MountainTour()
        {
            this.HasGuide = false;
        }
        public MountainTour(Guid id, string name, double price, string desc, TimeSpan dur, DateTime start, bool hasGuide)
            : base(id, name, price, desc, dur, start)
        {
            this.HasGuide = hasGuide;
        }
    }
}
