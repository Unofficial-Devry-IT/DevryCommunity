using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<CodeSnippet> CodeSnippets { get; set; }
        DbSet<CodeInfo> CodeInfo { get; set; }
        DbSet<Channel> Channels { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}