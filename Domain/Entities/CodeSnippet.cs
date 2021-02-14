using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Events.CodeSnippets;

namespace Domain.Entities
{
    public class CodeSnippet : AuditableEntity, IHasDomainEvent
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Category { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public string CodeInfoId { get; set; }
        public CodeInfo CodeInfo { get; set; }

        private bool _done;

        public bool Done
        {
            get => _done;

            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new CodeSnippetCreatedEvent(this));
                }

                _done = value;
            }
        }

        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}