using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using travel_agency_system.Interfaces;

namespace travel_agency_system.Models
{
    public enum TourActivity { Guide, Beach, Spa, Skiing }

    public enum FilterCategory {All, Price, Date }

    public enum SortOrder { Ascending, Descending }

    public class TravelPackage: Entity, ISearchable,IFilterable<TourFilterOptions>, ISortable<TravelPackage, TourFilterOptions>
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }= null;
        public TimeSpan Duration { get; set; }
        public DateTime StartDate { get; set; }

        public List<TourActivity> Activities { get; set; } = new();

        public TravelPackage()
        {
            this.Name = string.Empty;
            this.Price = 0;
            this.Description = string.Empty;
            this.Duration = TimeSpan.Zero;
            this.StartDate = DateTime.MinValue;
            this.Activities = new();

        }

        public TravelPackage(string? name, double price, string? description, TimeSpan duration, DateTime startDate)
            : base()
        {
            this.Name = name;
            this.Price = price;
            this.Description = description;
            this.Duration = duration;
            this.StartDate = startDate;
            this.Activities = new();
        }

        public TravelPackage( string? name, double price, string? description, TimeSpan duration, DateTime startDate, List<TourActivity> activities)
            : this(name, price, description, duration, startDate) 
        {
            this.Activities = activities ?? new();
        }

        public override bool IsValid()
        {
            bool isIdValid = base.IsValid();
            return isIdValid &&
                   !string.IsNullOrEmpty(Name) &&
                   Price > 0 &&
                   Duration.TotalMinutes > 0 &&
                   StartDate > DateTime.MinValue;
        }

        public void AddActivity(TourActivity activity)
        {
            if (Activities == null)
            {
                Activities = new List<TourActivity>();
            }
            if (!Activities.Contains(activity))
            {
                Activities.Add(activity);
            }
        }

        public bool Matches(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) { return true; }

            return (this.Name != null && this.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ||
                (Description != null && Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }


        public bool IsMatch(TourFilterOptions options)
        {
            if (options.Category == FilterCategory.Price)
            {
                if (double.TryParse(options.MinValue, out double min) && Price < min) return false;
                if (double.TryParse(options.MaxValue, out double max) && Price > max) return false;
            }

            if (options.Category == FilterCategory.Date || !string.IsNullOrEmpty(options.MinValue) || !string.IsNullOrEmpty(options.MaxValue))
            {
                if (DateTime.TryParse(options.MinValue, out DateTime minDate) && StartDate.Date < minDate.Date)
                    return false;

                if (DateTime.TryParse(options.MaxValue, out DateTime maxDate))
                {
                    var endDate = StartDate.Add(Duration).Date;
                    if (endDate > maxDate.Date) return false;
                }
            }

            return true;
        }

        public IEnumerable<TravelPackage> ApplySort(IEnumerable<TravelPackage> items, TourFilterOptions options)
        {
            return (options.Category, options.Order) switch
            {
                (FilterCategory.Price, SortOrder.Ascending) => items.OrderBy(t => t.Price),
                (FilterCategory.Price, SortOrder.Descending) => items.OrderByDescending(t => t.Price),
                (FilterCategory.Date, SortOrder.Ascending) => items.OrderBy(t => t.StartDate),
                (FilterCategory.Date, SortOrder.Descending) => items.OrderByDescending(t => t.StartDate),
                _ => items
            };
        }
    }
}
