namespace eVote470Plus.Core.Application.Dtos.Results
{
    public class ElectionValidationResult
    {
        public bool CanCreate { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }
}
