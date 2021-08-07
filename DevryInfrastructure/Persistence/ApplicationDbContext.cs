using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevryDomain.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UnofficialDevryIT.Architecture.Scheduler;
using System.Linq;

namespace DevryInfrastructure.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser>, IApplicationDbContext, IScheduleDbContext
    {
        public ApplicationDbContext(DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            
        }

        public DbSet<Reminder> Reminders { get; set; }
        
        #region Gamification Related
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeResponse> ChallengeResponses { get; set; }
        public DbSet<GamificationCategory> GamificationCategories { get; set; }
        public DbSet<GamificationEntry> GamificationEntries { get; set; }
        #endregion
        
        public async Task<TSchedule> AddSchedule<TSchedule>(TSchedule task) where TSchedule : class, IScheduledTask
        {
            await Set<TSchedule>().AddAsync(task);
            await SaveChangesAsync();

            return task;
        }

        public async Task<TSchedule> DeleteSchedule<TSchedule>(TSchedule task) where TSchedule : class, IScheduledTask
        {
            Set<TSchedule>().Remove(task);
            await SaveChangesAsync();

            return task;
        }

        public async Task<IList<TSchedule>> GetScheduledItems<TSchedule>() where TSchedule : class, IScheduledTask
        {
            if (typeof(TSchedule) == typeof(IScheduledTask))
            {
                return await Reminders
                    .Select(x => x as TSchedule)
                    .ToListAsync();
            }
            
            try
            {
                return await Set<TSchedule>().ToListAsync();
            }
            catch
            {
                return new List<TSchedule>();
            }
        }

        public async Task<IList<TSchedule>> GetScheduledTimesInRange<TSchedule>(DateTime @from, DateTime to) where TSchedule : class, IScheduledTask
        {
            if (typeof(TSchedule) == typeof(IScheduledTask))
                return new List<TSchedule>();
            
            return await Set<TSchedule>()
                .Where(x => x.NextRunTime >= from)
                .Where(x => x.NextRunTime <= to)
                .OrderByDescending(x => x.NextRunTime)
                .ToListAsync();
        }

        public async Task UpdateSchedule<T>(T task) where T : class, IScheduledTask
        {
            Set<T>().Update(task);
            await SaveChangesAsync();
        }
    }
}