using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Services
{
    public static class IdGenerator
    {
        public static Guid Generate() => Guid.NewGuid();
    }
}
