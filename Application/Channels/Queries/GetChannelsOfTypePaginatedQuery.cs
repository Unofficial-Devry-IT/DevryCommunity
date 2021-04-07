using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Extensions;
using Domain.Enums;
using MediatR;

namespace Application.Channels.Queries
{
    public class GetChannelsOfTypePaginatedQuery : IRequest<PaginatedList<ChannelResponse>>
    {
        public ChannelType ChannelType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetChannelsOfTypePaginatedQueryHandler : IRequestHandler<GetChannelsOfTypePaginatedQuery,
            PaginatedList<ChannelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetChannelsOfTypePaginatedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ChannelResponse>> Handle(GetChannelsOfTypePaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _context.Channels
                .Where(x => x.ChannelType == request.ChannelType)
                .Select(x=>new ChannelResponse()
                {
                    Id = x.Id.ToString(),
                    GuildId = x.GuildId.ToString(),
                    Description = x.Description,
                    Name = x.Name,
                    Position = x.Position,
                    ChannelType = x.ChannelType
                })
                .OrderBy(x => x.Position)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}