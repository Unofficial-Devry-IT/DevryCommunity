using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Entities;
using Domain.Events.Reminders;
using MediatR;

namespace Application.Reminders.Commands
{
    /// <summary>
    /// Create reminders which act like notifications from the bot
    /// <see cref="CreateReminderCommandHandler"/> handles these requests
    /// </summary>
    public class CreateReminderCommand : IRequest<string>
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
    }

    /// <summary>
    /// Handles <see cref="CreateReminderCommand"/> requests
    /// </summary>
    public class CreateReminderCommandHandler : IRequestHandler<CreateReminderCommand, string>
    {
        private readonly IApplicationDbContext _context;

        public CreateReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateReminderCommand request, CancellationToken cancellationToken)
        {
            /*
              To ensure information is not lost/improperly received - the clients base64 encode certain values (as seen below)
              So, we need to reverse that so we can properly store those values              
             */
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

            // notify the architecture that a reminder was created
            entity.DomainEvents.Add(new ReminderCreatedEvent(entity));

            await _context.Reminders.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id.ToString();
        }
    }
}