using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using travel_agency_system.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace travel_agency_system.Models
{
    public enum TourActivity { Guide, Beach, Spa, Skiing }

    public enum FilterCategory {All, Price, Date }

    public enum SortOrder { Ascending, Descending }
    [Table("TravelPackages")]
    public class TravelPackage: Entity, ISearchable,IFilterable<TourFilterOptions>, ISortable<TravelPackage, TourFilterOptions>
    {
        [Required]
        [MaxLength(150)]
        public string? Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }= null;
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public DateTime StartDate { get; set; }

        public List<TourActivity> Activities { get; set; } = new();

        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        public TravelPackage()
        {
            this.Name = string.Empty;
            this.Price = 0m;
            this.Description = string.Empty;
            this.Duration = TimeSpan.Zero;
            this.StartDate = DateTime.MinValue;
            this.Activities = new();

        }

        public TravelPackage(string? name, double price, string? description, TimeSpan duration, DateTime startDate)
            : base()
        {
            this.Name = name;
            this.Price = (decimal)price;
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

            return new[] { Name, Description }.Any(prop => prop?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false);
        }


        public bool IsMatch(TourFilterOptions options)
        {
            if (options.Category == FilterCategory.Price)
            {
                if (decimal.TryParse(options.MinValue, out decimal min) && Price < min) return false;
                if (decimal.TryParse(options.MaxValue, out decimal max) && Price > max) return false;
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
                (FilterCategory.Price, SortOrder.Ascending) => items.OrderBy(t => t.Price).ThenBy(t => t.StartDate),
                (FilterCategory.Price, SortOrder.Descending) => items.OrderByDescending(t => t.Price).ThenByDescending(t => t.StartDate),
                (FilterCategory.Date, SortOrder.Ascending) => items.OrderBy(t => t.StartDate).ThenBy(t => t.Name),
                (FilterCategory.Date, SortOrder.Descending) => items.OrderByDescending(t => t.StartDate).ThenByDescending(t => t.Name),
                _ => items.OrderBy(t => t.Name).ThenBy(t => t.Price)
            };
        }
    }
}
