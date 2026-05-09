using eVote470Plus.Core.Application.ViewModels.People.Candidate;

namespace eVote470Plus.Core.Application.ViewModels.Political.Election
{
    public class ElectionResultViewModel
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public List<CandidateResultViewModel> Candidates { get; set; } = new();
    }
}
