using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using travel_agency_system.Models;

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

    }
}
