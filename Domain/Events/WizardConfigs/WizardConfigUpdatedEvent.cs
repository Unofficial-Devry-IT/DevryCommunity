using Domain.Entities.Configs;

namespace Domain.Events.WizardConfigs
{
    public class WizardConfigUpdatedEvent : BaseEvent<WizardConfig>
    {
        public WizardConfigUpdatedEvent(WizardConfig config) : base(config)
        {
        }
    }
}