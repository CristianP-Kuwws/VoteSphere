using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Domain.Entities.People
{
    public class Candidate 
    {
        public int CandidateId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public required bool IsActive { get; set; } 
        public int PoliticalPartyId { get; set; }

        // Navigation properties

        public PoliticalParty PoliticalParty { get; set; }
        public ICollection<CandidatePosition>? CandidatePositions { get; set; }
        public ICollection<Vote>? Votes { get; set; }
    }
}
