using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public sealed class UserManager
    {
        private static UserManager? _instance;
        private static readonly object _lock = new object();

        private List<Admin> _adminsCache = new();
        private List<Customer> _customersCache = new();

        public User? CurrentUser { get; private set; }

        private UserManager() { }

        public static UserManager GetInstance
        {
            get { lock (_lock) return _instance ??= new UserManager(); }
        }

        public void SetUser(User user) => CurrentUser = user;

        public void Logout() => CurrentUser = null;
        public bool IsAdmin => CurrentUser is Admin;

        public bool HasAdmin => _adminsCache.Count > 0;
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
            using (var context = new AppDbContext())
            {
                context.Customers.Update(updatedCustomer);
                await context.SaveChangesAsync();
            }

            var index = _customersCache.FindIndex(c => c.Email == updatedCustomer.Email);
            if (index != -1)
            {
                _customersCache[index] = updatedCustomer;
                if (CurrentUser is Customer current && current.Email == updatedCustomer.Email)
                {
                    CurrentUser = updatedCustomer;
                }
            }
        }

        public async Task InitializeAsync()
        {
            using (var context = new AppDbContext())
            {
                _adminsCache = await context.Admins.AsNoTracking().ToListAsync();
                _customersCache = await context.Customers.AsNoTracking().ToListAsync();
            }
        }
        public async Task AddAdminAsync(Admin admin)
        {
            using (var context = new AppDbContext())
            {
                context.Admins.Add(admin);
                await context.SaveChangesAsync();
            }

            _adminsCache.Add(admin);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            using (var context = new AppDbContext())
            {
                context.Customers.Add(customer);
                await context.SaveChangesAsync();
            }
            _customersCache.Add(customer);
        }
    }
}