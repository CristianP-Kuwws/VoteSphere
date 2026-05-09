namespace eVote470Plus.Core.Domain.Entities.Political
{
    public class PoliticalPosition
    {
        public int PoliticalPositionId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }

    }
}
