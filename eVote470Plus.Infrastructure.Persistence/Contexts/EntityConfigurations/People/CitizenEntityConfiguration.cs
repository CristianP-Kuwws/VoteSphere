using eVote470Plus.Core.Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eVote470Plus.Infrastructure.Persistence.Contexts.EntityConfigurations.People
{
    public class CitizenEntityConfiguration : IEntityTypeConfiguration<Citizen>
    {
        public void Configure(EntityTypeBuilder<Citizen> builder)
        {
            builder.HasKey(c => c.CitizenId);
            builder.ToTable("Citizens");

            builder.Property(c => c.Name).IsRequired().HasMaxLength(60);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(60);
            builder.Property(c => c.DocumentNumber).IsRequired().HasMaxLength(11);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.Property(c => c.IsActive).IsRequired();

            // Index

            builder.HasIndex(c => c.DocumentNumber).IsUnique();

            // Relationships

            builder.HasMany(c => c.Votes)
                   .WithOne(v => v.Citizen)
                   .HasForeignKey(v => v.CitizenId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
