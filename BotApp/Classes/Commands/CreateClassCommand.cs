using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using BotApp.Helpers;
using Domain.Entities.Discord;
using MediatR;

namespace BotApp.Classes.Commands
{
    public class CreateClassCommand : IRequest
    {
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public string CourseCategory { get; set; }
        public string Description { get; set; }
        public string[] VoiceChannels { get; set; }
        public string[] TextChannels { get; set; }

        public override string ToString()
        {
            return $"Course Number: {CourseNumber}\n" +
                   $"Course Name: {CourseName}\n" +
                   $"Course Category: {CourseCategory}\n" +
                   $"Description: {Description}\n" +
                   $"Voice Channels: {(VoiceChannels != null ? string.Join(", ", VoiceChannels) : "")}\n" +
                   $"Text Channels: {(TextChannels != null ? string.Join(",", TextChannels) : "")}\n";
        }
    }

    public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand>
    {
        private readonly IApplicationDbContext _context;

        public CreateClassCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            await ChannelHelper.CreateClass(request);
            return Unit.Value;
        }
    }
}