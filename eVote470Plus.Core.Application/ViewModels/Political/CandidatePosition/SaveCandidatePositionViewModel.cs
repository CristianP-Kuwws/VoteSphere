using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Political.CandidatePosition
{
    public class SaveCandidatePositionViewModel
    {
        public int CandidatePositionId { get; set; }

        [Required(ErrorMessage = "The Candidate is required.")]
        public int CandidateId { get; set; }

        [Required(ErrorMessage = "The Political Position is required.")]
        public int PoliticalPositionId { get; set; }
        public int PoliticalPartyId { get; set; } // Obtained automatically

    }
}
