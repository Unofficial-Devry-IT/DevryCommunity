using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Configs.Queries
{
    public class GetConfigQuery : IRequest<Config>
    {
        public string Id { get; set; }
    }

    public class GetConfigQueryHandler : IRequestHandler<GetConfigQuery, Config>
    {
        private readonly IApplicationDbContext _context;

        public GetConfigQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Config> Handle(GetConfigQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Configs.FindAsync(request.Id);
        }
    }
}