using System;

namespace Domain.Common.Models
{
    /// <summary>
    /// For tracking purposes of who modified/created what and when
    /// </summary>
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    }
}