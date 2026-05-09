using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Domain.Entities.People
{
    public class PoliticalLeader 
    {
        public string PoliticalLeaderId { get; set; }
        public string IdentityUserId { get; set; }
        public int PoliticalPartyId { get; set; }

        // Navigation properties
        public PoliticalParty? PoliticalParty { get; set; }
    }
}
