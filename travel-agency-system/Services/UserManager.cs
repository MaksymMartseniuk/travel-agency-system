using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using travel_agency_system.Models;
using System.Drawing.Imaging.Effects;

namespace travel_agency_system.Services
{
    public sealed class UserManager
    {
        private static UserManager? _instance;
        private static readonly object _lock = new object();
        public User? CurrentUser { get; private set; }

        private UserManager() { }

        public static UserManager GetInstance
        {
            get { lock (_lock) return _instance ??= new UserManager(); }
        }
        public void SetUser(User user) => CurrentUser = user;

        public void Logout() => CurrentUser = null;
        public bool IsAdmin => CurrentUser is Admin;
        public bool IsLoggedIn => CurrentUser != null;

        public async Task UpdateCustomerAsync(Customer updatedCustomer)
        {
            var storage = StorageService.GetInstance;
            string fileName = storage.GetFileName<Customer>();

            var customers = await storage.LoadFromFileAsync<Customer>(fileName);

            var index = customers.FindIndex(c => c.Email == updatedCustomer.Email);
            if (index != -1)
            {
                customers[index] = updatedCustomer;
                await storage.SaveToFileAsync(fileName, customers);
            }
        }
    }
}
