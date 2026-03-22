using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public enum TourActivity { Guide, Beach, Spa, Skiing }
    public class TravelPackage: Entity
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

        public new bool IsValid()
        {
            bool isIdValid = base.IsValid();
            return isIdValid &&
                   !string.IsNullOrEmpty(Name) &&
                   Price > 0 &&
                   Duration.TotalMinutes > 0 &&
                   StartDate > DateTime.MinValue;
        }
        public override string GetInfo() => $"Тур: {Name} - {Price}$";

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
    }
}
