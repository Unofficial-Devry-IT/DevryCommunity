using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace DevryApplication.Common.Interfaces
{
    /// <summary>
    /// Information / Tables required by the architecture
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<Reminder> Reminders { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}