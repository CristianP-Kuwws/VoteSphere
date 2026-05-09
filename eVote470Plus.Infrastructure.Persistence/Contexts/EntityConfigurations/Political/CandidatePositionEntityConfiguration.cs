using eVote470Plus.Core.Domain.Entities.Political;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Political
{
    public class CandidatePositionEntityConfiguration : IEntityTypeConfiguration<CandidatePosition>
    {
        public void Configure(EntityTypeBuilder<CandidatePosition> builder)
        {
            builder.HasKey(cp => cp.CandidatePositionId);
            builder.ToTable("CandidatePositions");

            //Relationships

            builder.HasOne(cp => cp.Candidate)
                    .WithMany(c => c.CandidatePositions)
                    .HasForeignKey(cp => cp.CandidateId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.PoliticalPosition)
                    .WithMany()
                    .HasForeignKey(cp => cp.PoliticalPositionId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.PoliticalParty)
                .WithMany() 
                .HasForeignKey(cp => cp.PoliticalPartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
