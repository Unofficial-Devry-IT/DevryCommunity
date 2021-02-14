using System;

namespace Domain.Entities.Configs
{
    [Serializable]
    public class WizardToCommandLink
    {
        public string DiscordCommand { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}