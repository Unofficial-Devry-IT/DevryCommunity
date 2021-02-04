using DevryService.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database
{
    using Models.Configs;

    public class DevryDbContext : DbContext
    {
        public DevryDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            
        }

        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<CommandConfig> CommandConfigs { get; set; }
        public DbSet<WizardConfig> WizardConfigs { get; set; }
        public DbSet<CodeInfo> CodeInfo { get; set; }
    }
}
