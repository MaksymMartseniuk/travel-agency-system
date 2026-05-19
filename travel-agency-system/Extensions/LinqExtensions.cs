using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> data, int pageNumber, int pageSize)
        {
            return data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
