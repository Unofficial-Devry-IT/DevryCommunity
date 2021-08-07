using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace DevryInfrastructure.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Reminder> Reminders { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}