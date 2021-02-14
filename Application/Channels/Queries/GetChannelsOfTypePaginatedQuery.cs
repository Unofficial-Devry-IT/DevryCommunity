using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Channels.Queries
{
    public class GetChannelsOfTypePaginatedQuery : IRequest<PaginatedList<Channel>>
    {
        public ChannelType ChannelType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetChannelsOfTypePaginatedQueryHandler
        : IRequestHandler<GetChannelsOfTypePaginatedQuery, PaginatedList<Channel>>
    {
        private readonly IApplicationDbContext _context;
        private IMapper _mapper;

        public GetChannelsOfTypePaginatedQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<Channel>> Handle(GetChannelsOfTypePaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _context.Channels
                .Where(x => x.ChannelType == request.ChannelType)
                .OrderBy(x => x.Position)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}