using MediatR;

namespace BotApp.Classes.Commands
{
    public class CreatePublicAreaCommand : IRequest
    {
        public string Name { get; set; }
    }
}