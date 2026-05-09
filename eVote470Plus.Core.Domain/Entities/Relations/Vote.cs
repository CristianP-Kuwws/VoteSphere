using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Domain.Entities.Relations
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int CitizenId { get; set; }
        public int? CandidateId { get; set; } 
        public int ElectionId { get; set; }
        public int PoliticalPositionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Citizen Citizen { get; set; }
        public Candidate Candidate { get; set; }
        public Election Election { get; set; }
        public PoliticalPosition PoliticalPosition { get; set; }
    }
}
