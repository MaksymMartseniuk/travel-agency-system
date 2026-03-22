using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TourBuilder : ITourBuilder
    {
        private TravelPackage? _tour;

        public TourBuilder() 
        {
            _tour = new TravelPackage();
        }

        public ITourBuilder AddActivity(TourActivity activity)
        {
            _tour?.AddActivity(activity);
            return this;
        }

        public TravelPackage Build()
        {
            var result = _tour;
            _tour = new TravelPackage();
            return result;
        }

        public ITourBuilder SetBaseInfo(string name, double price, string description)
        {
            _tour?.Name=name;
            _tour?.Price=price;
            _tour?.Description=description;
            return this;
        }

        public ITourBuilder SetTiming(DateTime startDate, int durationDays)
        {
            _tour?.StartDate=startDate;
            _tour?.Duration = TimeSpan.FromDays(durationDays);
            return this;
        }
    }
}
