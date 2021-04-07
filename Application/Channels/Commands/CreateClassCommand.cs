using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Discord;
using Domain.Enums;
using Domain.Events.Channels;
using MediatR;

namespace Application.Channels.Commands
{
    /// <summary>
    /// Information that must be provided when creating a channel
    /// <see cref="CreateClassCommandHandler"/> handles the request
    /// </summary>
    public class CreateClassCommand : IRequest<ulong>
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string ChannelName { get; set; }
        public ChannelType ChannelType { get; set; }
        public int Position { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Handles what happens when a <see cref="CreateClassCommand"/> request is made
    /// </summary>
    public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ulong>
    {
        private readonly IApplicationDbContext _context;

        public CreateClassCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ulong> Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            // Convert request into channel
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

            // Propagate the event across the system/architecture
            // Anyone listening can consume this event and do what they need to do
            entity.DomainEvents.Add(new ChannelCreatedEvent(entity));

            // Finally - add and save changes to the database
            await _context.Channels.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            return entity.Id;
        }
    }
}