namespace eVote470Plus.Core.Application.ViewModels.People.Candidate
{
    public class CandidateResultViewModel
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateLastName { get; set; }
        public string PartyName { get; set; }
        public string PartyAcronym { get; set; }
        public string? PhotoUrl { get; set; }
        public int VoteCount { get; set; }
        public decimal VotePercentage { get; set; }
    }
}
