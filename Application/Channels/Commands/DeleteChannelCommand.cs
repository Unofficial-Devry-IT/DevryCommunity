using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Discord;
using Domain.Events.Channels;
using Domain.Exceptions;
using MediatR;
using Microsoft.VisualBasic.CompilerServices;

namespace Application.Channels.Commands
{
    /// <summary>
    /// When user wants to remove/delete channel
    /// <see cref="DeleteChannelCommandHandler"/> handles this request
    /// </summary>
    public class DeleteChannelCommand : IRequest
    {
        public ulong Id { get; set; }
    }

    /// <summary>
    /// Handles <see cref="DeleteChannelCommand"/> requests
    /// </summary>
    public class DeleteChannelCommandHandler : IRequestHandler<DeleteChannelCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteChannelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteChannelCommand request, CancellationToken cancellationToken)
        {
            // Is the architecture aware of this channel?
            var entity = await _context.Channels.FindAsync(request.Id);
            
            if (entity == null)
                throw new NotFoundException(nameof(Channel), request.Id);

            // Remove the channel from the architecture
            _context.Channels.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}