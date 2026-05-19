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
                var filtered = source.Where(item =>
                    item != null &&
                    item.IsValid() &&
                    item.Matches(searchQuery) &&
                    item.IsMatch(options)
                );
                var sorted = new T();
                return sorted.ApplySort(filtered, options).ToList();

            });


            OnDataProcessed?.Invoke(processedData);
        }

    }
}
