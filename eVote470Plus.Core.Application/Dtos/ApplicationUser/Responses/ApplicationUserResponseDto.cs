namespace eVote470Plus.Core.Application.Dtos.ApplicationUser
{
    public class ApplicationUserResponseDto
    {
        public bool HasError { get; set; }
        public required List<string>? Errors { get; set; }

    }
}
