namespace eVote470Plus.Core.Application.ViewModels.Political.PoliticalPosition
{
    public class PoliticalPositionViewModel
    {
        public int PoliticalPositionId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
    }
}
