namespace eVote470Plus.Core.Application.Dtos.Relations
{
    public class VoteDto
    {
        public int VoteId { get; set; }
        public int CitizenId { get; set; }
        public int CandidateId { get; set; }
        public int ElectionId { get; set; }
        public int PoliticalPositionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
