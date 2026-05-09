using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Core.Domain.Entities.Political;

namespace eVote470Plus.Core.Domain.Entities.Relations
{
    public class PoliticalAlliance 
    {
        public int PoliticalAllianceId { get; set; }
        public int SenderPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public AllianceStatus Status { get; set; }
        public DateTime? ResponseDate { get; set; } 

        // Navigation properties
        public PoliticalParty SenderParty { get; set; }
        public PoliticalParty ReceiverParty { get; set; }
    }
}
