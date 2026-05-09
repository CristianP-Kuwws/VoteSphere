namespace eVote470Plus.Core.Application.Dtos.People
{
    public class CitizenDto
    {
        public int CitizenId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string DocumentNumber { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
    }
}
