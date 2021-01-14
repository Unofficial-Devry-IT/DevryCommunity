using DevryServices.Common.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryServices.Common
{
    public static class GlobalConfiguration
    {
        public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();
        public static string WebRootPath { get; set; }
        public static string ContentRootPath { get; set; }
    }
}
