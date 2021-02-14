using System.Collections.Generic;

namespace Domain.Entities.Configs
{
    public class EmbedConfig : MessageConfig
    {
        public string Title { get; set; }
        public string Footer { get; set; }
        public List<string> Fields { get; set; }
    }
}