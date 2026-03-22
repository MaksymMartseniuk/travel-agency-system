using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;

namespace travel_agency_system.Interfaces
{
    public interface ITourBuilder
    {
        ITourBuilder SetBaseInfo(string name,double price,string description);
        ITourBuilder SetTiming(DateTime startDate, int durationDays);
        ITourBuilder AddActivity(TourActivity activity);
        TravelPackage Build();

    }
}
