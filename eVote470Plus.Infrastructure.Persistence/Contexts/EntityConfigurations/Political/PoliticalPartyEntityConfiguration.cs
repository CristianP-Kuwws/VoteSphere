using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Entities.Political;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Political
{
    public class PoliticalPartyEntityConfiguration : IEntityTypeConfiguration<PoliticalParty>
    {
        public void Configure(EntityTypeBuilder<PoliticalParty> builder)
        {
            builder.HasKey(pp => pp.PoliticalPartyId);
            builder.ToTable("PoliticalParties");

            builder.Property(pp => pp.Name).IsRequired().HasMaxLength(60);
            builder.Property(pp => pp.Description).HasMaxLength(500);
            builder.Property(pp => pp.Acronym).IsRequired().HasMaxLength(10);
            builder.Property(pp => pp.IsActive).IsRequired();
            builder.Property(pp => pp.LogoUrl).HasMaxLength(255);

            // Index

            builder.HasIndex(pp => pp.Acronym).IsUnique();

            // Relationships 

            builder.HasMany(pp => pp.Candidates)
                   .WithOne(c => c.PoliticalParty)
                   .HasForeignKey(c => c.PoliticalPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pp => pp.SentAlliances)
                   .WithOne(pa => pa.SenderParty)
                   .HasForeignKey(pa => pa.SenderPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pp => pp.ReceivedAlliances)
                     .WithOne(pa => pa.ReceiverParty)
                     .HasForeignKey(pa => pa.ReceiverPartyId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pp => pp.PoliticalLeader)
                     .WithOne(pl => pl.PoliticalParty)
                     .HasForeignKey<PoliticalLeader>(pl => pl.PoliticalPartyId) 
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
