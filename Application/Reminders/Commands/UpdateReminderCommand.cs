using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Reminders.Commands
{
    public class UpdateReminderCommand : IRequest
    {
        public string Id { get; set; }
        public string Schedule { get; set; }
        public string Name { get; set; }
    }

    public class UpdateReminderCommandHandler : IRequestHandler<UpdateReminderCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateReminderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateReminderCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Reminders.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Reminder), request.Id);

            entity.Name = request.Name;
            entity.Schedule = request.Schedule;

            _context.Reminders.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}