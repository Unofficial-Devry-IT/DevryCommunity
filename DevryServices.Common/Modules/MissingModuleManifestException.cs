using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Modules
{
    public class MissingModuleManifestException : Exception
    {
        public string ModuleName { get; set; }
        public MissingModuleManifestException() { }
        public MissingModuleManifestException(string message) : base(message) { }
        public MissingModuleManifestException(string message, string moduleName) : this(message)
        {
            ModuleName = moduleName;
        }
    }
}
