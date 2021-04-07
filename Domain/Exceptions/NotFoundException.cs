using System;

namespace Domain.Exceptions
{
    /// <summary>
    /// When something is not found this exception shall get thrown
    /// </summary>
    public class NotFoundException : Exception 
    {
        public NotFoundException() : base()
        {
        }

        public NotFoundException(string message) : base(message)
        {
            
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            
        }

        public NotFoundException(string name, object key)
            : base($"Entity: \"{name}\" ({key}) was not found")
        {
            
        }
    }
}