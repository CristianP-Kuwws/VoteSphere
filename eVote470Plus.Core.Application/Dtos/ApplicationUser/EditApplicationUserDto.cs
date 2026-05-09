namespace eVote470Plus.Core.Application.Dtos.People
{
    public class EditApplicationUserDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? Password { get; set; }
        public required bool IsActive { get; set; }
        public required string Role { get; set; }

    }
}
