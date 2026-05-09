namespace eVote470Plus.Core.Application.Dtos.Political
{
    public class ElectionDto
    {
        public int ElectionId { get; set; }
        public required string Name { get; set; }
        public DateTime ElectionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public bool IsActive { get; set; }

        // Summary statistics
        public int? ParticipantPoliticalPartiesCount { get; set; }
        public int? PoliticalPositionsCount { get; set; }
        public decimal? VoterTurnoutPercentage { get; set; }
    }
}
