using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Entities;
using Domain.Events.Reminders;
using Domain.Exceptions;
using MediatR;

namespace Application.Reminders.Commands
{
    /// <summary>
    /// Update a reminder's information
    /// <see cref="UpdateReminderCommandHandler"/> handles these requests
    /// </summary>
    public class UpdateReminderCommand : IRequest
    {
        public string Id { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Handles <see cref="UpdateReminderCommand"/> requests
    /// </summary>
    public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
        {
            // Ensure the architecture is tracking this reminder
            var entity = await _context.Reminders.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Reminder), request.Id);

            /*
                 Certain values are encoded by the client for data integrity 
                 We must reverse this to properly read/save the data
             */
            entity.Name = request.Name.FromBase64();
            entity.Schedule = request.Schedule.FromBase64();

            // Propagate this event across the architecture
            entity.DomainEvents.Add(new ReminderUpdatedEvent(entity));
            
            _context.Reminders.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}