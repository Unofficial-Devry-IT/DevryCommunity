using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Reminders.Queries
{
    public class GetRemindersPaginatedQuery : IRequest<PaginatedList<Reminder>>
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetRemindersPaginatedQueryHandler
        : IRequestHandler<GetRemindersPaginatedQuery, PaginatedList<Reminder>>
    {
        private readonly IApplicationDbContext _context;
        private IMapper _mapper;

        public GetRemindersPaginatedQueryHandler(IApplicationDbContext context,
            IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PaginatedList<Reminder>> Handle(GetRemindersPaginatedQuery request, CancellationToken cancellationToken)
        {
            return await _context.Reminders
                .Where(x => x.GuildId == request.GuildId && x.ChannelId == request.ChannelId)
                .OrderByDescending(x => x.NextRunTime)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}