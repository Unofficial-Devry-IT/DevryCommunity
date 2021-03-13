using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands
{
    public class UpdateRoleCommand : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Roles.FirstOrDefaultAsync(x =>
                x.Id.ToString().Equals(request.Id, StringComparison.CurrentCulture));

            if (entity == null)
                throw new NotFoundException(nameof(Role), request.Id);

            entity.Name = request.Name;
            entity.Description = request.Description;

            _context.Roles.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}