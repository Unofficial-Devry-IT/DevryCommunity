using System.Threading.Tasks;

namespace BotApp.Services
{
    public interface IConfigService
    {
        Task InitializeInteractionConfigs();

        Task InitializeCommandConfigs();
    }
}