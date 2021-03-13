using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Reminders.Queries;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Roles.Queries
{
    public class GetRolesPaginatedQuery : IRequest<PaginatedList<Role>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetRemindersPaginatedQueryHandler
        : IRequestHandler<GetRolesPaginatedQuery, PaginatedList<Role>>
    {
        private readonly IApplicationDbContext _context;
        private IMapper _mapper;

        public GetRemindersPaginatedQueryHandler(IApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedList<Role>> Handle(GetRolesPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Roles
                .OrderByDescending(x => x.Name)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}