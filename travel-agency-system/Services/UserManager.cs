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
        
        public async Task<bool> LoginAsync(string email, string password)
        {
            var storage = StorageService.GetInstance;
            string hashedInput = PasswordHasher.HashPassword(password);
            var customers = await storage.LoadFromFileAsync<Customer>(storage.GetFileName<Customer>());
            var customer = customers.FirstOrDefault(c => c.Email == email && c.PasswordHash == hashedInput);
            if (customer != null)
            {
                CurrentUser = customer;
                return true;
            }
            var admins = await storage.LoadFromFileAsync<Admin>(storage.GetFileName<Admin>());
            var admin = admins.FirstOrDefault(a => a.Email == email && a.PasswordHash == hashedInput);
            if (admin != null)
            {
                CurrentUser = admin;
                return true;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var storage = StorageService.GetInstance;
            var admins = await storage.LoadFromFileAsync<Admin>(storage.GetFileName<Admin>());
            var customers = await storage.LoadFromFileAsync<Customer>(storage.GetFileName<Customer>());
            if (admins.Any(a => a.Email == email) || customers.Any(c => c.Email == email))
            {
                return false;
            }
            string hashedPassword = PasswordHasher.HashPassword(password);
            if (admins.Count == 0)
            {
                var newAdmin = new Admin(email, hashedPassword,true);
                if (!newAdmin.IsValid()) return false;
                admins.Add(newAdmin);
                await storage.SaveToFileAsync(storage.GetFileName<Admin>(), admins);
            }
            else
            {
                double balance = 0.0;
                var newCustomer = new Customer(email, hashedPassword,balance);
                if (!newCustomer.IsValid()) return false;
                customers.Add(newCustomer);
                await storage.SaveToFileAsync(storage.GetFileName<Customer>(), customers);
            }

            return true;

        }
        public void Logout()
        {
            CurrentUser = null;
        }


    }
}
