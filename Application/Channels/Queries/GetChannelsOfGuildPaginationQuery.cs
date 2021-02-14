using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Channels.Queries
{
    public class GetChannelsWithPaginationQuery : IRequest<PaginatedList<Channel>>
    {
        public ulong GuildId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetChannelsWithPaginationQueryHandler 
        : IRequestHandler<GetChannelsWithPaginationQuery, PaginatedList<Channel>>
    {
        private readonly IApplicationDbContext _context;
        private IMapper _mapper;

        public GetChannelsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<Channel>> Handle(GetChannelsWithPaginationQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Channels
                .Where(x => x.GuildId == (ulong) request.GuildId)
                .OrderBy(x => x.Position)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}