using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class AuthService
    {
        private readonly StorageService _storage = StorageService.GetInstance;
        private readonly UserManager _session = UserManager.GetInstance;
        public async Task<bool> LoginAsync(string email, string password)
        {
            string hashedInput = PasswordHasher.HashPassword(password);
            var admins = await _storage.LoadFromFileAsync<Admin>(_storage.GetFileName<Admin>());
            var customers = await _storage.LoadFromFileAsync<Customer>(_storage.GetFileName<Customer>());

            User? user = admins.FirstOrDefault(a => a.Email == email && a.PasswordHash == hashedInput)
                         ?? (User?)customers.FirstOrDefault(c => c.Email == email && c.PasswordHash == hashedInput);
            if (user != null)
            {
                _session.SetUser(user);
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            var admins = await _storage.LoadFromFileAsync<Admin>(_storage.GetFileName<Admin>());
            var customers = await _storage.LoadFromFileAsync<Customer>(_storage.GetFileName<Customer>());
            if (admins.Any(a => a.Email == email) || customers.Any(c => c.Email == email))
                return false;

            string hashedPassword = PasswordHasher.HashPassword(password);

            if (admins.Count == 0)
            {
                var newAdmin = new Admin(email, hashedPassword, true);
                if (!newAdmin.IsValid()) return false;

                admins.Add(newAdmin);
                await _storage.SaveToFileAsync(_storage.GetFileName<Admin>(), admins);
            }
            else
            {
                var newCustomer = new Customer(email, hashedPassword, 0.0);
                if (!newCustomer.IsValid()) return false;

                customers.Add(newCustomer);
                await _storage.SaveToFileAsync(_storage.GetFileName<Customer>(), customers);
            }

            return true;
        }
    }
}
