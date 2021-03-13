using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Entities.Configs;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<CodeSnippet> CodeSnippets { get; set; }
        DbSet<CodeInfo> CodeInfo { get; set; }
        DbSet<Channel> Channels { get; set; }

        DbSet<WizardConfig> WizardConfigs { get; set; }
        DbSet<CommandConfig> CommandConfigs { get; set; }
        DbSet<Reminder> Reminders { get; set; }
        DbSet<Role> Roles { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}