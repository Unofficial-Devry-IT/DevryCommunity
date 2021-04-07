using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Reminders.Commands
{
    /// <summary>
    /// Remove reminders from service
    /// <see cref="DeleteReminderCommandHandler"/> handles these requests
    /// </summary>
    public class DeleteReminderCommand : IRequest
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// Handles <see cref="DeleteReminderCommand"/> requests
    /// </summary>
    public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
        {
            // Ensure the reminder's ID is being tracked by the application
            var entity = await _context.Reminders.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Reminder), request.Id);

            _context.Reminders.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}