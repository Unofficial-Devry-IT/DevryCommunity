using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Extensions;
using Domain.Entities.Discord;
using MediatR;

namespace Application.Channels.Queries
{
    public class GetChannelsOfGuildPaginatedQuery : IRequest<PaginatedList<Channel>>
    {
        public ulong GuildId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetChannelsWithGuildPaginatedQueryHandler : IRequestHandler<GetChannelsOfGuildPaginatedQuery,
            PaginatedList<Channel>>
    {
        private readonly IApplicationDbContext _context;

        public GetChannelsWithGuildPaginatedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Channel>> Handle(GetChannelsOfGuildPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Channels
                .Where(x => x.GuildId == (ulong) request.GuildId)
                .OrderBy(x => x.Position)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}