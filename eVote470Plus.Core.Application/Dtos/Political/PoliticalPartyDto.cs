namespace eVote470Plus.Core.Application.Dtos.Political
{
    public class PoliticalPartyDto
    {
        public int PoliticalPartyId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Acronym { get; set; }
        public required bool IsActive { get; set; }
        public string? LogoUrl { get; set; }
    }
}
