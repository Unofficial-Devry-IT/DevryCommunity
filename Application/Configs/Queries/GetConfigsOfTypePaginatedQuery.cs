using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Extensions;
using Domain.Entities;
using MediatR;

namespace Application.Configs.Queries
{
    public class GetConfigsOfTypePaginatedQuery : IRequest<PaginatedList<Config>>
    {
        public ConfigType ConfigType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetConfigsOfTypePaginatedQueryHandler : IRequestHandler<GetConfigsOfTypePaginatedQuery, PaginatedList<Config>>
    {
        private readonly IApplicationDbContext _context;

        public GetConfigsOfTypePaginatedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Config>> Handle(GetConfigsOfTypePaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _context.Configs.Where(x =>
                    x.ConfigType.Equals(request.ConfigType))
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}