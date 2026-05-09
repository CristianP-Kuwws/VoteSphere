namespace eVote470Plus.Core.Domain.Entities.Political
{
    public class ElectionPosition
    {
        public int ElectionPositionId { get; set; }
        public int ElectionId { get; set; }
        public int PoliticalPositionId { get; set; }

        // Navigation properties
        public Election Election { get; set; }
        public PoliticalPosition PoliticalPosition { get; set; }
    }
}
