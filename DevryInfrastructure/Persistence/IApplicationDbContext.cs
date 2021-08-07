using System.Threading;
using System.Threading.Tasks;
using DevryDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace DevryInfrastructure.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Reminder> Reminders { get; set; }
        DbSet<Challenge> Challenges { get; set; }
        DbSet<ChallengeResponse> ChallengeResponses { get; set; }
        DbSet<GamificationCategory> GamificationCategories { get; set; }
        DbSet<GamificationEntry> GamificationEntries { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}