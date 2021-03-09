using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Channels.Queries
{
    public class GetAllChannelsPaginationQuery : IRequest<PaginatedList<ChannelResponse>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ChannelResponse
    {
        public string Id { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ChannelType ChannelType { get; set; }
        public int Position { get; set; }
    }

    public class GetAllChannelsPaginationQueryHandler 
        : IRequestHandler<GetAllChannelsPaginationQuery, PaginatedList<ChannelResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllChannelsPaginationQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<ChannelResponse>> Handle(GetAllChannelsPaginationQuery request, CancellationToken cancellationToken)
        {
            return await _context.Channels.Select(x=>new ChannelResponse()
                {
                    Id = x.Id.ToString(),
                    GuildId = x.GuildId.ToString(),
                    Name = x.Name,
                    Description = x.Description,
                    Position = x.Position,
                    ChannelType = x.ChannelType
                })
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}