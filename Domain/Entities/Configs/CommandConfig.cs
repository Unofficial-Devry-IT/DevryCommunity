using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Newtonsoft.Json;

namespace Domain.Entities.Configs
{
    public class CommandConfig : CommonConfig, IHasDomainEvent
    {
        public string DiscordCommand { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public string RestrictedRolesJSON { get; protected set; }

        [NotMapped]
        public List<string> RestrictedRoles
        {
            get => JsonConvert.DeserializeObject<List<string>>(RestrictedRolesJSON);
            set => JsonConvert.SerializeObject(value);
        }
        
        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                _done = value;
            }
        }
        
        [NotMapped]
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}