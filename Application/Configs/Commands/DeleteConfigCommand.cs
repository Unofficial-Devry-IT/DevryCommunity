using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Configs.Commands
{
    public class DeleteConfigCommand : IRequest
    {
        public string Id { get; set; }
    }

    public class DeleteConfigCommandHandler : IRequestHandler<DeleteConfigCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteConfigCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Configs.FindAsync(request.Id, cancellationToken);

            if (entity != null)
            {
                _context.Configs.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}