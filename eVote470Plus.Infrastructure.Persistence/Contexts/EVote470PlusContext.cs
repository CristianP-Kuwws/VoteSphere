using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Entities.Relations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace eVote470Plus.Infrastructure.Persistence.Contexts
{
    public class EVote470PlusContext : DbContext
    {
        public EVote470PlusContext(DbContextOptions<EVote470PlusContext> options) : base(options) { }

        // People

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet <PoliticalLeader> PoliticalLeaders { get; set; }

        // Political

        public DbSet<CandidatePosition> CandidatePositions { get; set; }
        public DbSet<Election> Elections { get; set; }
        public DbSet<ElectionPosition> ElectionPositions { get; set; }
        public DbSet<PoliticalParty> PoliticalParties { get; set; }
        public DbSet<PoliticalPosition> PoliticalPositions { get; set; }

        // Relations

        public DbSet<PoliticalAlliance> PoliticalAlliances { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
