using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace travel_agency_system.Services
{
    public sealed class StorageService
    {
        private static StorageService? _instance;
        private static readonly object _lock = new object();

        private StorageService() { }
        public static StorageService GetInstance
        {
            get { lock (_lock) return _instance ??= new StorageService(); }
        }

        public async Task SaveToFileAsync<T>(string fileName, List<T> data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(fileName, jsonString);
        }
        public async Task<List<T>> LoadFromFileAsync<T>(string fileName)
        {
            if (!File.Exists(fileName)) return new List<T>();
            string jsonString = await File.ReadAllTextAsync(fileName);
            return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
        }

    }
}
