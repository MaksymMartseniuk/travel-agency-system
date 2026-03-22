using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class TourManager
    {
        private readonly StorageService _storage = StorageService.GetInstance;

        public async Task<List<TravelPackage>> GetAllToursAsync()
        {
            return await _storage.LoadFromFileAsync<TravelPackage>(_storage.GetFileName<TravelPackage>());
        }

        public async Task AddTourAsync(TravelPackage newTour)
        {
            var tours = await GetAllToursAsync();
            tours.Add(newTour);
            await _storage.SaveToFileAsync(_storage.GetFileName<TravelPackage>(), tours);
        }
    }
}
