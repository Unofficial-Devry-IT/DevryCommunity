using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities.Configs
{
    public class CommandConfig : CommonConfig, IHasDomainEvent
    {
        public string DiscordCommand { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public List<string> RestrictedRoles { get; set; } = new List<string>();
        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                _done = value;
            }
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}