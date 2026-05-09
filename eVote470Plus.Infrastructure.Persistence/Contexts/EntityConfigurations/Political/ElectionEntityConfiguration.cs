using eVote470Plus.Core.Domain.Entities.Political;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Political
{
    public class ElectionEntityConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            builder.HasKey(e => e.ElectionId);
            builder.ToTable("Elections");

            builder.Property(e => e.Name).IsRequired().HasMaxLength(60);
            builder.Property(e => e.ElectionDate).IsRequired();
            builder.Property(e => e.FinishedAt).IsRequired(false);
            builder.Property(e => e.IsActive).IsRequired();

            //Relationships

            builder.HasMany(e => e.ElectionPositions)
                   .WithOne(ep => ep.Election)
                   .HasForeignKey(ep => ep.ElectionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Votes)
                    .WithOne(v => v.Election)
                    .HasForeignKey(v => v.ElectionId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
