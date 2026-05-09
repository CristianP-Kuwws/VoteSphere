namespace eVote470Plus.Core.Application.Dtos.Political
{
    public class CandidatePositionDto
    {
        public int CandidatePositionId { get; set; }
        public int CandidateId { get; set; }
        public int PoliticalPositionId { get; set; }
        public int PoliticalPartyId { get; set; }
    }
}
