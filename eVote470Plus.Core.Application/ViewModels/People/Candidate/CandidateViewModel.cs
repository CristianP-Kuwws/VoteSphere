namespace eVote470Plus.Core.Application.ViewModels.People.Candidate
{
    public class CandidateViewModel
    {
        public int CandidateId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public required bool IsActive { get; set; }

        // Extended properties
        public string PoliticalPartyPosition { get; set; } = "No Position Associated";
    }
}
