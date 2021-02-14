using Domain.Common;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Channels.Commands
{
    using Common.Interfaces;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;
    
    public class DeleteChannelCommand : IRequest
    {
        public ulong Id { get; set; }
    }

    public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteChannelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Channels.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Channel), request.Id);

            _context.Channels.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}