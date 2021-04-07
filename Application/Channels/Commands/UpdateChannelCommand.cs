using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Discord;
using Domain.Events.Channels;
using Domain.Exceptions;
using MediatR;

namespace Application.Channels.Commands
{
    /// <summary>
    /// When channel information needs to be updated
    /// <see cref="UpdateChannelCommandHandler"/> handles this request
    /// </summary>
    public class UpdateChannelCommand : IRequest
    {
        public ulong Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }

    /// <summary>
    /// Handles <see cref="UpdateChannelCommand"/> requests
    /// </summary>
    public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateChannelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            // Is the architecture aware of this channel
            var entity = await _context.Channels.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Channel), request.Id);

            // Move info from the request into the entity for us to update/save
            entity.Description = request.Description;
            entity.Name = request.Title;
            entity.Position = request.Position;
            entity.LastModified = DateTime.Now;

            entity.DomainEvents.Add(new ChannelUpdatedEvent(entity));
            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}