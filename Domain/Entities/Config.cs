using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Models;

namespace Domain.Entities
{
    /// <summary>
    /// Configuration for whatever is needed within the architecture
    /// </summary>
    public class Config : IHasDomainEvent, IExtendableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Name of config
        /// </summary>
        public string ConfigName { get; set; }
        
        public ConfigType ConfigType { get; set; } = ConfigType.INTERACTION;

        [NotMapped] 
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

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