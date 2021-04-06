using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandConfigs.Queries
{
    public class GetConfigQuery : IRequest<Config>
    {
        public string ConfigName { get; set; }
    }

    public class GetCommandConfigQueryHandler : IRequestHandler<GetConfigQuery, Config>
    {
        private readonly IApplicationDbContext _context;

        public GetCommandConfigQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Config> Handle(GetConfigQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Configs.FirstOrDefaultAsync(x =>
                x.ConfigName.Equals(request.ConfigName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}