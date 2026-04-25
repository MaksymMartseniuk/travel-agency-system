using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;
using travel_agency_system.Services;

namespace travel_agency_system.Interfaces
{
    public interface IDataManager<T, TOptions> where T : class, IEntity, ISearchable, IFilterable<TOptions>, ISortable<T, TOptions>, new()
    {
        event DataProcessedHandler<T>? OnDataProcessed;
        Task ProcessAsync(IEnumerable<T> source, string searchQuery, TOptions options);
    }
}
