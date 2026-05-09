namespace eVote470Plus.Core.Application.ViewModels.Relations.Vote
{
    public class VotingPositionViewModel
    {
        public int PoliticalPositionId { get; set; }
        public string Name { get; set; }
        public int CandidateCount { get; set; }
        public int PartyCount { get; set; }
        public bool AlreadyVoted { get; set; }
    }

}
