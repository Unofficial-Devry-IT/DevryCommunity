using DevryServices.Common.Models;

namespace Domain.Entities.Configs.ConfigTypes
{
    public class WizardConfig : CommandConfig, IExtendableObject
    {
        public string Headline { get; set; }
        public bool AcceptAnyUser { get; set; } = false;
        public bool RequireMention { get; set; } = false;
        public string ExtensionData { get; set; }
    }
}