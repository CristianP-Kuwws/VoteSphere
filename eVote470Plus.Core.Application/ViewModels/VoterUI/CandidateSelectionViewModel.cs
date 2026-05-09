using eVote470Plus.Core.Application.ViewModels.VoterUI;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.VoteScreen
{
    public class CandidateSelectionViewModel
    {
        public int PoliticalPositionId { get; set; }
        public string PoliticalPositionName { get; set; }

        public List<CandidateOptionViewModel> Candidates { get; set; }

        [Required(ErrorMessage = "A Candidate must be selected first.")]
        public int SelectedCandidateId { get; set; }
    }

}
