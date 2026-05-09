namespace eVote470Plus.Core.Application.ViewModels.Political.Election
{
    public class ElectionViewModel
    {
        public int ElectionId { get; set; }
        public required string Name { get; set; }
        public DateTime ElectionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public bool IsActive { get; set; }

        // Extended properties
        public int? ParticipantPoliticalPartiesCount { get; set; }
        public int? PoliticalPositionsCount { get; set; }
        public double? VoterTurnoutPercentage { get; set; } // For results page
    }
}
