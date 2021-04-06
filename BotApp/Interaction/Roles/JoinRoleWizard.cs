using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;

namespace BotApp.Interaction.Roles
{
    public class JoinRoleWizard : WizardBase
    {
        public JoinRoleWizard(CommandContext context) : base(context)
        {
        }

        public override string WizardName() => "Sorting Hat";

        protected override async Task ExecuteAsync(CommandContext context)
        {
            JsonDocument doc = JsonDocument.Parse(CurrentConfig.ExtensionData);
            JsonElement element = doc.RootElement.GetProperty("BlacklistedRoles");
            
            string[] blacklistedRoles = element.EnumerateArray().Select(x => x.GetString())
                .ToArray();
            
            
        }
    }
}