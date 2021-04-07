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
    public class GetAllConfigsPaginatedQuery : IRequest<PaginatedList<Config>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class
        GetAllConfigsPaginatedQueryHandler : IRequestHandler<GetAllConfigsPaginatedQuery, PaginatedList<Config>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllConfigsPaginatedQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Config>> Handle(GetAllConfigsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Configs.PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}