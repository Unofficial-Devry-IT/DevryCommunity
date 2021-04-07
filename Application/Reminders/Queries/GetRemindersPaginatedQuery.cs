using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Extensions;
using Domain.Entities;
using MediatR;

namespace Application.Reminders.Queries
{
    public class GetRemindersPaginatedQuery : IRequest<PaginatedList<Reminder>>
    {
        public ulong ChannelId { get; set; }
        public ulong GuildId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class
        GetRemindersPaginatedQueryHandler : IRequestHandler<GetRemindersPaginatedQuery, PaginatedList<Reminder>>
    {
        private readonly IApplicationDbContext _context;

        public GetRemindersPaginatedQueryHandler(IApplicationDbContext context)
        {
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