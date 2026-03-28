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
        private List<Admin> _adminsCache = new();
        private List<Customer> _customersCache = new();

        private readonly StorageService _storage = StorageService.GetInstance;
        public User? CurrentUser { get; private set; }

        private UserManager() { }

        public static UserManager GetInstance
        {
            get { lock (_lock) return _instance ??= new UserManager(); }
        }
        public void SetUser(User user) => CurrentUser = user;

        public void Logout() => CurrentUser = null;
        public bool IsAdmin => CurrentUser is Admin;

        public bool HasAdmin=> _adminsCache.Count > 0;
        public bool IsLoggedIn => CurrentUser != null;

        public User? this[string email]
        {
            get
            {
                User? user = _adminsCache.FirstOrDefault(a => a.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                return user ?? _customersCache.FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            }
        }

        public async Task UpdateCustomerAsync(Customer updatedCustomer)
        {
            var index = _customersCache.FindIndex(c => c.Email == updatedCustomer.Email);

            if (index != -1)
            {
                _customersCache[index] = updatedCustomer;
                await _storage.SaveToFileAsync(_storage.GetFileName<Customer>(), _customersCache);
                if (CurrentUser is Customer current && current.Email == updatedCustomer.Email)
                {
                    CurrentUser = updatedCustomer;
                }
            }
        }

        public async Task InitializeAsync()
        {
            _adminsCache = await _storage.LoadFromFileAsync<Admin>(_storage.GetFileName<Admin>()) ?? new List<Admin>();
            _customersCache = await _storage.LoadFromFileAsync<Customer>(_storage.GetFileName<Customer>()) ?? new List<Customer>();
        }

        public async Task AddAdminAsync(Admin admin)
        {
            var storage = StorageService.GetInstance;
            _adminsCache.Add(admin);
            await _storage.SaveToFileAsync(_storage.GetFileName<Admin>(), _adminsCache);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            var storage = StorageService.GetInstance;
            _customersCache.Add(customer);
            await _storage.SaveToFileAsync(_storage.GetFileName<Customer>(), _customersCache);
        }
    }
}
