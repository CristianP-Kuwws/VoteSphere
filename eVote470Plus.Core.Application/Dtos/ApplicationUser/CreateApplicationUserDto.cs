namespace eVote470Plus.Core.Application.Dtos.People
{
    public class CreateApplicationUserDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required bool IsActive { get; set; } = true;
        public required string Role { get; set; }

    }
}
