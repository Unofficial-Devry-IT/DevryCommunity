using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Extensions;
using Domain.Entities;
using MediatR;

namespace Application.Configs.Commands
{
    public class UpdateConfigCommand : IRequest
    {
        public string Id { get; set; }
        public string ConfigName { get; set; }
        public ConfigType ConfigType { get; set; }
        public string Data { get; set; }
    }

    public class UpdateConfigCommandHandler : IRequestHandler<UpdateConfigCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateConfigCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Configs.FindAsync(request.Id, cancellationToken);
            
            if(entity == null)
                return Unit.Value;

            entity.ConfigName = request.ConfigName;
            entity.ConfigType = request.ConfigType;
            entity.ExtensionData = request.Data.FromBase64();

            _context.Configs.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}