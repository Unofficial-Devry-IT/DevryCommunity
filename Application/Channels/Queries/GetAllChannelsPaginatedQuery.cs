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
    /// <summary>
    /// User wants to retrieve all channels in a paginated format
    /// </summary>
    public class GetAllChannelsPaginatedQuery : IRequest<PaginatedList<ChannelResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Unfortunately javaScript cannot handle <see cref="ulong"/> primitive types.
    /// So all <see cref="ulong"/> references are swapped for <see cref="string"/> values
    /// </summary>
    public class ChannelResponse
    {
        public string Id { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelType ChannelType { get; set; }
        public int Position { get; set; }
    }
    
    /// <summary>
    /// Handles <see cref="GetAllChannelsPaginatedQuery"/> requests
    /// </summary>
    public class
        GetAllChanelsPaginatedQueryHandler : IRequestHandler<GetAllChannelsPaginatedQuery,
            PaginatedList<ChannelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllChanelsPaginatedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ChannelResponse>> Handle(GetAllChannelsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            // The following is a conversion from Channel to ChannelResponse - as listed above for JavaScript limitations 
            return await _context.Channels.Select(x => new ChannelResponse()
            {
                Id = x.Id.ToString(),
                GuildId = x.GuildId.ToString(),
                Name = x.Name,
                Description = x.Description,
                Position = x.Position,
                ChannelType = x.ChannelType
            }).PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}