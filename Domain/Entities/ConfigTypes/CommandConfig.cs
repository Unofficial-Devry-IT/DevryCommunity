using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Domain.Entities.ConfigTypes
{
    public class CommandConfig : CommonConfig
    {
        public string DiscordCommand { get; set; }
        public bool IgnoreHelpWizard { get; set; }
        public string RestrictedRolesJSON { get; protected set; } = "[]";

        [NotMapped]
        public List<string> RestrictedRoles
        {
            get => JsonConvert.DeserializeObject<List<string>>(RestrictedRolesJSON);
            set => RestrictedRolesJSON = JsonConvert.SerializeObject(value);
        }
    }
}