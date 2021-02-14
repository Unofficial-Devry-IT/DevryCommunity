using System;
using System.Collections.Generic;
using DevryServices.Common.Models;
using Domain.Common;
using Domain.Events.CodeInfo;

namespace Domain.Entities
{
    public class CodeInfo : EntityBase, IHasDomainEvent
    {
        public CodeInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public string Language { get; set; }
        public string Color { get; set; }
        public string FileExtension { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                if (value && !_done)
                    DomainEvents.Add(new CodeInfoCreatedEvent(this));

                _done = value;
            }
        }
    }
}