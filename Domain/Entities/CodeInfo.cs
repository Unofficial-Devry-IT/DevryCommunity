using System;

namespace Domain.Entities
{
    public class CodeInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Language { get; set; }
        public string Color { get; set; }
        public string FileExtension { get; set; }
    }
}