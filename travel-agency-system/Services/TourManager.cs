using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;
using travel_agency_system.Models;
using Microsoft.EntityFrameworkCore;

namespace travel_agency_system.Services
{
    public class TourManager
    {
        public async Task<List<TravelPackage>> GetAllToursAsync()
        {
            using (var context = new AppDbContext())
            {
                return await context.TravelPackages.ToListAsync();
            }
        }

        public async Task AddTourAsync(TravelPackage newTour)
        {
            using (var context = new AppDbContext())
            {
                context.TravelPackages.Add(newTour);
                await context.SaveChangesAsync();
            }
        }

        public async Task SaveAllToursAsync(List<TravelPackage> tours)
        {
            using (var context = new AppDbContext())
            {
                context.TravelPackages.UpdateRange(tours);
                await context.SaveChangesAsync();
            }
        }
    }
}
