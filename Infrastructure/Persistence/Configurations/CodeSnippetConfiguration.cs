using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CodeSnippetConfiguration : IEntityTypeConfiguration<CodeSnippet>
    {
        public void Configure(EntityTypeBuilder<CodeSnippet> builder)
        {
            builder.Ignore(x => x.DomainEvents);
            builder.HasIndex(x => x.Id);
        }
    }
}