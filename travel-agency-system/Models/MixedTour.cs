using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class MixedTour:TravelPackage
    {
        public string? BeachType { get; set; }
        public bool HasGuide { get; set; }
        public MixedTour()
        {
            HasGuide = false;
            BeachType= string.Empty;
        }
        public MixedTour(Guid id, string name, double price, string desc, TimeSpan dur, DateTime start, string beachType, bool hasGuide)
            : base(id, name, price, desc, dur, start)
        {
            this.BeachType = beachType;
            this.HasGuide = hasGuide;
        }

        public override string GetInfo()
        {
            string guideStr = HasGuide ? "Гід включений" : "Гіда немає";
            return $"{base.GetInfo()} | Змішаний тур: {BeachType}, {guideStr}";
        }
    }
}
