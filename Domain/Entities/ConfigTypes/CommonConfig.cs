using System;

namespace Domain.Entities.ConfigTypes
{
    public class CommonConfig
    {
        public TimeSpan? TimeoutOverride { get; set; } = null;
        public string AuthorName { get; set; }
        public string AuthorIcon { get; set; }
        public string ReactionEmoji { get; set; }
        public string Description { get; set; }
    }
}