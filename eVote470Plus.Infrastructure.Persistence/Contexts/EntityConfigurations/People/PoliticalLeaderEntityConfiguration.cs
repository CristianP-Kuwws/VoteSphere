using eVote470Plus.Core.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.People
{
    public class PoliticalLeaderEntityConfiguration : IEntityTypeConfiguration<PoliticalLeader>
    {
        public void Configure(EntityTypeBuilder<PoliticalLeader> builder)
        {
            builder.HasKey(pl => pl.PoliticalLeaderId);
            builder.ToTable("PoliticalLeaders");

            builder.Property(pl => pl.IdentityUserId).IsRequired();
            builder.Property(pl => pl.PoliticalPartyId).IsRequired();

            builder.Property(pl => pl.PoliticalLeaderId).ValueGeneratedOnAdd();

            // Relationships

            builder.HasOne(pl => pl.PoliticalParty)
                    .WithOne(pp => pp.PoliticalLeader)
                    .HasForeignKey<PoliticalLeader>(pl => pl.PoliticalPartyId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
