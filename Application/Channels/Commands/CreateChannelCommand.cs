using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Events.Channels;
using MediatR;

namespace Application.Channels.Commands
{
    public class CreateChannelCommand : IRequest<ulong>
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
        public ChannelType ChannelType { get; set; }
        public int Position { get; set; }
        public string Description { get; set; }
    }

    public class CreateChannelCommandHandler : IRequestHandler<CreateChannelCommand, ulong>
    {
        private readonly IApplicationDbContext _context;

        public CreateChannelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ulong> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
        {
            var entity = new Channel()
            {
                Created = DateTime.Now,
                Name = request.ChannelName,
                GuildId = request.GuildId,
                Id = request.ChannelId,
                ChannelType = request.ChannelType,
                Position = request.Position,
                Description = request.Description
            };

            entity.DomainEvents.Add(new ChannelCreatedEvent(entity));

            await _context.Channels.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            
            return entity.Id;
        }
    }
}