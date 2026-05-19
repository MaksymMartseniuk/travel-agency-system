using System;
using System.Collections.Generic;
using System.Text;
using travel_agency_system.Interfaces;
using travel_agency_system.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace travel_agency_system.Models
{
    public abstract class Entity:IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Entity ()
        {
            this.Id = IdGenerator.Generate();
        }

        public Entity(Guid id)
        {
            this.Id = id;
        }
        public virtual bool IsValid()
        {
            return Id != Guid.Empty;
        }

    }
}
