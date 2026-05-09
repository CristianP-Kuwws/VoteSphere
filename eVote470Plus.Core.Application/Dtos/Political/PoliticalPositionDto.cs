namespace eVote470Plus.Core.Application.Dtos.Political
{
    public class PoliticalPositionDto
    {
        public int PoliticalPositionId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
    }
}
