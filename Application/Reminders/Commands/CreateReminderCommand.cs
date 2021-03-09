using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Events.Reminders;
using MediatR;
using Application.Helpers;

namespace Application.Reminders.Commands
{
    public class CreateReminderCommand : IRequest<string>
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
    }

    public class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public CreateReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
        {
            string name = request.Name.FromBase64();
            string contents = request.Contents.FromBase64();
            string schedule = request.Schedule.FromBase64();
            
            var entity = new Reminder()
            {
                GuildId = request.GuildId,
                ChannelId = request.ChannelId,
                Schedule = schedule,
                Name = name,
                Contents = contents,
                NextRunTime = NCrontab.CrontabSchedule.Parse(schedule).GetNextOccurrence(DateTime.Now)
            };
            
            entity.DomainEvents.Add(new ReminderCreatedEvent(entity));

            await _context.Reminders.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}