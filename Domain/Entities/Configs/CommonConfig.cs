using System;
using DevryServices.Common.Models;

namespace Domain.Entities.Configs
{
    [Serializable]
    public abstract class CommonConfig : EntityBase, IExtendableObject
    {
        public TimeSpan? TimeoutOverride { get; set; } = null;

        public string AuthorName { get; set; }
        public string AuthorIcon { get; set; }
        public string ReactionEmoji { get; set; }
        public string ExtensionData { get; set; }
        public string Description { get; set; }
    }
}