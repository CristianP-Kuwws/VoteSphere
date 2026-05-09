using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Domain.Entities.Political
{
    public class PoliticalParty
    {
        public int PoliticalPartyId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Acronym { get; set; }
        public required bool IsActive { get; set; }
        public string? LogoUrl { get; set; }

        // Navigation properties
        public ICollection<Candidate>? Candidates { get; set; }
        public ICollection<PoliticalAlliance>? SentAlliances { get; set; }
        public ICollection<PoliticalAlliance>? ReceivedAlliances { get; set; }
        public PoliticalLeader? PoliticalLeader { get; set; }
    }
}
