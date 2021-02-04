using DevryServices.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    [Serializable]
    public abstract class CommonConfig : EntityBase, IExtendableObject
    {
        public TimeSpan? TimeoutOverride { get; set; } = null;

        public string AuthorName { get; set; }
        public string AuthorIcon { get; set; }
        public string Description { get; set; }
        public string ReactionEmoji { get; set; }
        public string ExtensionData { get; set; }
    }
}
