using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Reminders.Commands
{
    public class DeleteReminderCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteReminderCommandHandler : IRequestHandler<DeleteReminderCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteReminderCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Reminders.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Reminder), request.Id);

            _context.Reminders.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}