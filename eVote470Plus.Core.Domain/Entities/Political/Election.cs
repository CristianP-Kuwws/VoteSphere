using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Domain.Entities.Political
{
    public class Election
    {
        public int ElectionId { get; set; }
        public required string Name { get; set; }
        public DateTime ElectionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public ICollection<ElectionPosition>? ElectionPositions { get; set; }
        public ICollection<Vote>? Votes { get; set; }
    }
}
