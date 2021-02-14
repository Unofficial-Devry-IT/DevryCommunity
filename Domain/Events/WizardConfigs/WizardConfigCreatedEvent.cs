using Domain.Entities.Configs;

namespace Domain.Events.WizardConfigs
{
    public class WizardConfigCreatedEvent : BaseEvent<WizardConfig>
    {
        public WizardConfigCreatedEvent(WizardConfig config) : base(config)
        {
            
        }
    }
}