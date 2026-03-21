using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Security.Cryptography;

namespace travel_agency_system.Services
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using(var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
