using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Models
{
    public interface IExtendableObject
    {
        /// <summary>
        /// JSON Formatted string to extend the containing object.
        /// JSON Data can contain properties with arbitrary values (like primitives or complext objects).
        /// Extension methods are available <see cref="ExtendableObjectExtensions"/> to manipulate this data
        /// </summary>
        string ExtensionData { get; set; }
    }
}
