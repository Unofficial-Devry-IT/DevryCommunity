using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common.Models;

namespace Domain.Entities
{
    /// <summary>
    /// Dictates how information shall be displayed in Discord
    /// </summary>
    public class CodeInfo : EntityBase, IHasDomainEvent
    {
        /// <summary>
        /// Programming Language
        /// </summary>
        public string Language { get; set; }
        
        /// <summary>
        /// Color of embedded message in Discord
        /// </summary>
        public string Color { get; set; }
        
        /// <summary>
        /// Type of file this code information pertains to
        /// </summary>
        public string FileExtension { get; set; }

        [NotMapped] public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
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