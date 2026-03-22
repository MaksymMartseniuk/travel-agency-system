using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using travel_agency_system.Models;

namespace travel_agency_system.Converters
{
    public class ActivitiesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<TourActivity> activities)
            {
                string result = string.Join(", ", activities);
                return string.IsNullOrEmpty(result) ? "None" : result;
            }
            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
