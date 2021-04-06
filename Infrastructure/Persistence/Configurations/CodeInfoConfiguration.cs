using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CodeInfoConfiguration : IEntityTypeConfiguration<CodeInfo>
    {
        public void Configure(EntityTypeBuilder<CodeInfo> builder)
        {
            builder.Ignore(x => x.DomainEvents);
            builder.HasKey(x => x.Id);
        }
    }
}