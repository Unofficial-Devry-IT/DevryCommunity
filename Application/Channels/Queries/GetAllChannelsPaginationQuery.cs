using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Channels.Queries
{
    public class GetAllChannelsPaginationQuery : IRequest<PaginatedList<Channel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllChannelsPaginationQueryHandler 
        : IRequestHandler<GetAllChannelsPaginationQuery, PaginatedList<Channel>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllChannelsPaginationQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Channel>> Handle(GetAllChannelsPaginationQuery request, CancellationToken cancellationToken)
        {
            return await _context.Channels
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}