using System;
using System.Collections.Generic;
using System.Text;

namespace travel_agency_system.Models
{
    public class TravelPackage: Entity
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Description { get; set; }= null;
        public TimeSpan Duration { get; set; }
        public DateTime StartDate { get; set; }

        public static TravelPackage? ListHead;
        public TravelPackage? Next { get; set; }

        public TravelPackage()
        {
            AddToLinkedList();
        }

        public TravelPackage(Guid id, string? name, double price, string? description, TimeSpan duration, DateTime startDate)
            : base(id)
        {
            this.Name = name;
            this.Price = price;
            this.Description = description;
            this.Duration = duration;
            this.StartDate = startDate;

            AddToLinkedList();
        }

        private void AddToLinkedList()
        {
            this.Next = ListHead;
            ListHead = this;
        }
    }
}
