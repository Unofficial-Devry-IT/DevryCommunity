using System.IO;
using System.Threading.Tasks;
using UnofficialDevryIT.Architecture.Models;

namespace ChallengeAssistant.Interfaces
{
    public interface IChallengeParser
    {
        Task<ResultObject> ParseFileAsync(MemoryStream stream, string fileExtension);
    }
}