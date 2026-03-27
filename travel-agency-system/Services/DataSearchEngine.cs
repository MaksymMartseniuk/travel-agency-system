using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;
using System.Linq;

namespace travel_agency_system.Services
{
    public class DataSearchEngine<T> where T : ISearchable
    {
        public event Action<List<T>>? OnSearchCompleted;
        public void Search(IEnumerable<T> sourceData, string query)
        {
            if (sourceData == null) return;
            var result = sourceData.Where(item => item.Matches(query)).ToList();
            OnSearchCompleted?.Invoke(result);
        }
    }
}
