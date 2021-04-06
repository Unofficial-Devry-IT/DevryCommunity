using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using Domain.Exceptions;
using MediatR;

namespace Application.CommandConfigs.Commands
{
    public class UpdateConfigCommand : IRequest
    {
        public string Id { get; set; }
        public string Data { get; set; }
    }

    public class UpdateConfigHandler : IRequestHandler<UpdateConfigCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateConfigHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Configs.FindAsync(request.Id);

            if (entity == null)
                throw new NotFoundException(nameof(Config), request.Id);
            
            entity.ExtensionData = request.Data;

            _context.Configs.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}