using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Models;

namespace Domain.Entities
{
    /// <summary>
    /// Users shall be able to upload code snippets
    /// This entity is auditable
    /// </summary>
    public class CodeSnippet : AuditableEntity, IHasDomainEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Filterable item
        /// </summary>
        public string Category { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Code should be inserted in here
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Information on how this appears within Discord
        /// </summary>
        public string CodeInfoId { get; set; }
        public CodeInfo CodeInfo { get; set; }

        private bool _done;

        public bool Done
        {
            get => _done;

            set
            {
                _done = value;
            }
        }

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}