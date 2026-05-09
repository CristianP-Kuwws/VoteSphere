using eVote470Plus.Core.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.Relations
{
    public class PoliticalAllianceEntityConfiguration : IEntityTypeConfiguration<PoliticalAlliance>
    {
        public void Configure(EntityTypeBuilder<PoliticalAlliance> builder)
        {
            builder.HasKey(pa => pa.PoliticalAllianceId);
            builder.ToTable("PoliticalAlliances");

            builder.Property(pa => pa.SenderPartyId).IsRequired();
            builder.Property(pa => pa.ReceiverPartyId).IsRequired();
            builder.Property(pa => pa.Status).IsRequired().HasConversion<string>();
            builder.Property(pa => pa.ResponseDate).IsRequired(false);

            builder.HasOne(pa => pa.SenderParty)
                   .WithMany(pp => pp.SentAlliances)
                   .HasForeignKey(pa => pa.SenderPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pa => pa.ReceiverParty)
                     .WithMany(pp => pp.ReceivedAlliances)
                     .HasForeignKey(pa => pa.ReceiverPartyId)
                     .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
