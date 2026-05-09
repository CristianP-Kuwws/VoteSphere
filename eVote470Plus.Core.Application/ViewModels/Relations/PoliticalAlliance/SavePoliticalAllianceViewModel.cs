using eVote470Plus.Core.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Relations.PoliticalAlliance
{
    public class SavePoliticalAllianceViewModel
    {
        public int PoliticalAllianceId { get; set; }
        public int SenderPartyId { get; set; }

        [Required( ErrorMessage = "The Receiver Party is required.")]
        public int ReceiverPartyId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public AllianceStatus Status { get; set; } = AllianceStatus.Pending;
        public DateTime? ResponseDate { get; set; }

        // Extended Properties
        public string SenderPartyName { get; set; } = string.Empty; // + Their Acryonym (Not rquired but could be used for UI)
        public string ReceiverPartyName { get; set; } = string.Empty; // + Their Acryonym
    } 
}
