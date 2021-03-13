using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands
{
    public class DeleteRoleCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Roles
                .FirstOrDefaultAsync(x => x.Id.ToString().Equals(request.Id), cancellationToken);

            if (entity == null)
                throw new NotFoundException(nameof(Role), request.Id);

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}