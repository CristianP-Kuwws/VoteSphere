namespace eVote470Plus.Core.Application.Dtos.Political
{
    public class ElectionResultDto
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public List<CandidateResultDto> Candidates { get; set; }
    }
}
