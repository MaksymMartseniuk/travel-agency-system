using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;

namespace travel_agency_system.Services
{
    public class DataManager<T, TOptions> : IDataManager<T,TOptions> where T : class,IEntity,ISearchable,IFilterable<TOptions>, ISortable<T, TOptions>, new()
    {
        public event DataProcessedHandler<T>? OnDataProcessed;

        public async Task ProcessAsync(IEnumerable<T> source, string searchQuery, TOptions options)
        {
            var processedData = await Task.Run(() => {
                var filtered = GetProcessedSequence(source, searchQuery, options).ToList();
                var sorted = new T();
                return sorted.ApplySort(filtered, options).ToList();

            });


            OnDataProcessed?.Invoke(processedData);
        }

        private IEnumerable<T> GetProcessedSequence(IEnumerable<T> source, string query, TOptions options)
        {
            foreach (var item in source)
            {
                if (item != null && item.IsValid() && item.IsMatch(options) && item.Matches(query))
                {
                    yield return item;
                }
            }
        }
    }
}
