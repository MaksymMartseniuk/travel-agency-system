using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Models;

namespace travel_agency_system.Services
{
    public class AuthService
    {
        private readonly UserManager _session = UserManager.GetInstance;
        public async Task<bool> LoginAsync(string email, string password)
        {
            string hashedInput = PasswordHasher.HashPassword(password);

            User? user = _session[email];

            if (user != null && user.PasswordHash == hashedInput)
            {
                _session.SetUser(user);
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string email, string password)
        {
            
            if(_session[email] != null) return false;

            string hashedPassword = PasswordHasher.HashPassword(password);

            if (!_session.HasAdmin)
            {
                var newAdmin = new Admin(email, hashedPassword, true);
                if (!newAdmin.IsValid()) return false;

                await _session.AddAdminAsync(newAdmin);
            }
            else
            {
                var newCustomer = new Customer(email, hashedPassword, 0.0);
                if (!newCustomer.IsValid()) return false;

                await _session.AddCustomerAsync(newCustomer);
            }

            return true;
        }
    }
}
