namespace Domain.Entities.Configs
{
    public class WizardConfig : CommandConfig
    {
        public string Headline { get; set; }
        
        /// <summary>
        /// Shall the wizard accept any user, or only the user who initiated the call
        /// </summary>
        public bool AcceptAnyUser { get; set; } = false;
        
        /// <summary>
        /// Does the user have to include a mention to the bot in order for the bot to capture the response
        /// </summary>
        public bool RequireMention { get; set; } = false;
        public string CommandConfigId { get; set; }
        public CommandConfig CommandConfig { get; set; }
    }
}