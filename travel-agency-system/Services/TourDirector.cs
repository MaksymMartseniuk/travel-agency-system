using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TourDirector
    {
        private ITourBuilder? _builder;
        public TourDirector(ITourBuilder builder)
        {
            _builder = builder;
        }
        public void ChangeBuilder(ITourBuilder newBuilder)
        {
            _builder = newBuilder;
        }

        public void BuildWeekendCityBreak()
        {
            _builder?.SetBaseInfo("Weekend City Tour", 500.0, "Швидкий тур на вихідні з гідом.")
                    .SetTiming(DateTime.Now.AddDays(5), 2)
                    .AddActivity(TourActivity.Guide);
        }
        public void BuildLuxuryBeachVacation()
        {
            _builder?.SetBaseInfo("VIP Beach Vacation", 1200.0, "Розкішний відпочинок: пляж та SPA.")
                    .SetTiming(DateTime.Now.AddDays(14), 10)
                    .AddActivity(TourActivity.Beach)
                    .AddActivity(TourActivity.Spa);
        }
        public void BuildExtremeSkiingTour()
        {
            _builder?.SetBaseInfo("Extreme Skiing", 850.0, "Адреналін у горах.")
                    .SetTiming(DateTime.Now.AddMonths(2), 7)
                    .AddActivity(TourActivity.Skiing)
                    .AddActivity(TourActivity.Spa);
        }
    }
}
