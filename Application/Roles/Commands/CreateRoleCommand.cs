using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Helpers;
using Domain.Entities;
using Domain.Events.Roles;
using MediatR;

namespace Application.Roles.Commands
{
    public class CreateRoleCommand : IRequest<ulong>
    {
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ulong>
    {
        private readonly IApplicationDbContext _context;

        public CreateRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ulong> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            string name = request.Name.FromBase64();
            string description = request.Description.FromBase64();

            var entity = new Role()
            {
                GuildId = request.GuildId,
                Name = name,
                Description = description
            };

            entity.DomainEvents.Add(new RoleCreatedEvent(entity));

            await _context.Roles.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}