using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Channels.Commands
{
    public class UpdateChannelCommand : IRequest
    {
        public ulong Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }

    public class UpdateChannelCommandHandler : IRequestHandler<UpdateChannelCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateChannelCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Channels.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Channel), request.Id);

            entity.Description = request.Description;
            entity.Name = request.Title;
            entity.Position = request.Position;
            entity.LastModified = DateTime.Now;

            await _context.SaveChangesAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}