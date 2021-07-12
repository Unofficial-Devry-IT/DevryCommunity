using DevryApplication.Common.Interfaces;
using DevryDomain.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevryInfrastructure.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<IdentityUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            
        }

        public DbSet<Reminder> Reminders { get; set; }

        
    }
}