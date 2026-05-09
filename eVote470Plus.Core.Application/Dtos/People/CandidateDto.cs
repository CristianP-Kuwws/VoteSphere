namespace eVote470Plus.Core.Application.Dtos.People
{
    public class CandidateDto
    {
        public int CandidateId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? PhotoUrl { get; set; }
        public required bool IsActive { get; set; }
        public int PoliticalPartyId { get; set; }
    }
}
