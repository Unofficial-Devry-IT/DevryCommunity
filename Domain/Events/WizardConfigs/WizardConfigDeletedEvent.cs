using Domain.Entities.Configs;

namespace Domain.Events.WizardConfigs
{
    public class WizardConfigDeletedEvent : BaseEvent<WizardConfig>
    {
        public WizardConfigDeletedEvent(WizardConfig config) : base(config)
        {
            
        }
    }
}