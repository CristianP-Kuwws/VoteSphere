using eVote470Plus.Core.Domain.Entities.Political;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Political
{
    public class ElectionPositionEntityConfiguration : IEntityTypeConfiguration<ElectionPosition>
    {
        public void Configure(EntityTypeBuilder<ElectionPosition> builder)
        {
            builder.HasKey(ep => ep.ElectionPositionId);
            builder.ToTable("ElectionPositions");

            // Relationships

            builder.HasOne(ep => ep.Election)
                   .WithMany(e => e.ElectionPositions)
                   .HasForeignKey(ep => ep.ElectionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ep => ep.PoliticalPosition)
                     .WithMany()
                     .HasForeignKey(ep => ep.PoliticalPositionId)
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
