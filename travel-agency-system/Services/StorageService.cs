using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Threading.Tasks;

namespace travel_agency_system.Services
{
    public sealed class StorageService
    {
        private static StorageService? _instance;
        private static readonly object _lock = new object();

        private readonly string _dataFolder = "Data";
        private readonly JsonSerializerOptions _jsonOptions;

        private StorageService() 
        {
            if (!Directory.Exists(_dataFolder))
            {
                Directory.CreateDirectory(_dataFolder);
            }
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }
        public static StorageService GetInstance
        {
            get { lock (_lock) return _instance ??= new StorageService(); }
        }

        public async Task SaveToFileAsync<T>(string fileName, List<T> data)
        {
            try
            {
                string filePath = Path.Combine(_dataFolder, fileName);
                string jsonString = JsonSerializer.Serialize(data, _jsonOptions);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new IOException($"Помилка збереження у файл {fileName}: {ex.Message}");
            }
        }
        public async Task<List<T>> LoadFromFileAsync<T>(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_dataFolder, fileName);
                if (!File.Exists(filePath)) return new List<T>();

                string jsonString = await File.ReadAllTextAsync(filePath);
                return JsonSerializer.Deserialize<List<T>>(jsonString, _jsonOptions) ?? new List<T>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        public string GetFileName<T>() {
            return $"{typeof(T).Name}.json";
        }

    }
}
