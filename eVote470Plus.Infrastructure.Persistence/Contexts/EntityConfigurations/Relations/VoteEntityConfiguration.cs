using eVote470Plus.Core.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Relations
{
    public class VoteEntityConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasKey(v => v.VoteId);
            builder.ToTable("Votes");

            builder.Property(v => v.CreatedAt).IsRequired().HasDefaultValue(DateTime.UtcNow);

            // Relationships

            builder.HasOne(v => v.Citizen)
                   .WithMany(c => c.Votes)
                   .HasForeignKey(v => v.CitizenId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Candidate)
                     .WithMany(c => c.Votes)
                     .HasForeignKey(v => v.CandidateId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Election)
                        .WithMany(e => e.Votes)
                        .HasForeignKey(v => v.ElectionId)
                        .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.PoliticalPosition)
               .WithMany() 
               .HasForeignKey(v => v.PoliticalPositionId)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
