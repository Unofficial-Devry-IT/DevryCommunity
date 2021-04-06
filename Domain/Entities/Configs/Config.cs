using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DevryServices.Common.Models;
using Domain.Common;

namespace Domain.Entities.Configs
{
    public class Config : IHasDomainEvent, IExtendableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ConfigName { get; set; }

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; }
        
        public string ExtensionData { get; set; }

        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                _done = value;
            }
        }
    }
}