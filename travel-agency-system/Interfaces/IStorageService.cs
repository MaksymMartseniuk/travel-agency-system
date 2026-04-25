using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Interfaces
{
    public interface IStorageService
    {
        Task SaveToFileAsync<T>(string fileName, List<T> data) where T : class, IEntity, new();
        Task<List<T>> LoadFromFileAsync<T>(string fileName) where T : class, IEntity, new();
        string GetFileName<T>() where T : class, IEntity, new();
    }
}
