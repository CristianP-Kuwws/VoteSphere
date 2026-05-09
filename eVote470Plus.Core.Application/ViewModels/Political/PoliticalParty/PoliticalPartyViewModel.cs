namespace eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty
{
    public class PoliticalPartyViewModel
    {
        public int PoliticalPartyId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = "No description provided.";
        public required string Acronym { get; set; }
        public required bool IsActive { get; set; }
        public string? LogoUrl { get; set; }
    }
}
