using eVote470Plus.Core.Domain.Common.Enum;

namespace eVote470Plus.Core.Application.Dtos.Relations
{
    public class PoliticalAllianceDto
    {
        public int PoliticalAllianceId { get; set; }
        public int SenderPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public AllianceStatus Status { get; set; }
        public DateTime? ResponseDate { get; set; }
    }

}
