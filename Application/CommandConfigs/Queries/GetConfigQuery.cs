using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities.Configs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandConfigs.Queries
{
    public class GetCommandConfigQuery : IRequest<CommandConfig>
    {
        public string CommandName { get; set; }
    }

    public class GetCommandConfigQueryHandler : IRequestHandler<GetCommandConfigQuery, CommandConfig>
    {
        private readonly IApplicationDbContext _context;

        public GetCommandConfigQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandConfig> Handle(GetCommandConfigQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.CommandConfigs.FirstOrDefaultAsync(x =>
                x.DiscordCommand.Equals(request.CommandName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}