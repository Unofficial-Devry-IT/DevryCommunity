using Domain.Common.Models;

namespace Domain.Entities.ConfigTypes
{
    public class InteractionConfig : CommandConfig, IExtendableObject
    {
        public string Headline { get; set; }
        public bool AcceptAnyUser { get; set; } = false;
        public bool RequireMention { get; set; } = false;
        public string ExtensionData { get; set; }
    }
}