using eVote470Plus.Core.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.People
{
    public class CandidateEntityConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.HasKey(c => c.CandidateId);
            builder.ToTable("Candidates");

            builder.Property(c => c.Name).IsRequired().HasMaxLength(60);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.PhotoUrl).HasMaxLength(255);
            builder.Property(c => c.IsActive).IsRequired();

            // Relationships

            builder.HasOne(c => c.PoliticalParty)
                   .WithMany(p => p.Candidates)
                   .HasForeignKey(c => c.PoliticalPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.CandidatePositions)
                    .WithOne(p => p.Candidate)
                    .HasForeignKey(p => p.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Votes)
                    .WithOne(v => v.Candidate)
                    .HasForeignKey(v => v.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
