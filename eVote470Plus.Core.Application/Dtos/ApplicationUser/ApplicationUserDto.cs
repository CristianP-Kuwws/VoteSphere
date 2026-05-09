namespace eVote470Plus.Core.Application.Dtos.ApplicationUser
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; } 
        public required bool IsActive { get; set; }

    }
}
