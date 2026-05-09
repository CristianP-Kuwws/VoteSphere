using eVote470Plus.Core.Domain.Entities.Political;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Political
{
    public class PoliticalPositionEntityConfiguration : IEntityTypeConfiguration<PoliticalPosition>
    {
        public void Configure(EntityTypeBuilder<PoliticalPosition> builder)
        {
            builder.HasKey(pp => pp.PoliticalPositionId);
            builder.ToTable("PoliticalPositions");

            builder.Property(pp => pp.Name).IsRequired().HasMaxLength(60);
            builder.Property(pp => pp.Description).IsRequired().HasMaxLength(250);
            builder.Property(pp => pp.IsActive).IsRequired();
        }
    }
}
