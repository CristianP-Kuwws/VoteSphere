using eVote470Plus.Core.Domain.Common.Enum;

namespace eVote470Plus.Core.Application.ViewModels.Relations.PoliticalAlliance
{
    public class PoliticalAllianceViewModel
    {
        public int PoliticalAllianceId { get; set; }
        public int SenderPartyId { get; set; }
        public int ReceiverPartyId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public AllianceStatus Status { get; set; }
        public DateTime? ResponseDate { get; set; }

        // Extended Properties
        public string SenderPartyName { get; set; } = string.Empty; // + Their Acryonym
        public string ReceiverPartyName { get; set; } = string.Empty; // + Their Acryonym
    } 
}
