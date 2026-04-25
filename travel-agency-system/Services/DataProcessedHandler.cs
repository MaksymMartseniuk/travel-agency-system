using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;

namespace travel_agency_system.Services
{
    public delegate void DataProcessedHandler<T>(IEnumerable<T> results) where T : class, IEntity;
}
