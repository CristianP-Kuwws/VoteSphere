using eVote470Plus.Core.Domain.Entities.People;

namespace eVote470Plus.Core.Domain.Entities.Political
{
    public class CandidatePosition
    {
        public int CandidatePositionId { get; set; }
        public int CandidateId { get; set; }
        public int PoliticalPositionId { get; set; }
        public int PoliticalPartyId { get; set; }

        // Navigation properties

        public Candidate Candidate { get; set; }
        public PoliticalPosition PoliticalPosition { get; set; }
        public PoliticalParty PoliticalParty { get; set; }

    }
}
