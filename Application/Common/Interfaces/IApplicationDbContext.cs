using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Discord;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    /// <summary>
    /// Information / tables required by the architecture
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<CodeSnippet> CodeSnippets { get; set; }
        DbSet<CodeInfo> CodeInfo { get; set; }
        DbSet<Channel> Channels { get; set; }
        DbSet<Reminder> Reminders { get; set; }
        DbSet<Config> Configs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}