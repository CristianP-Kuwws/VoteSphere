namespace eVote470Plus.Core.Application.ViewModels.People.ApplicationUser
{
    public class ApplicationUserViewModel // (Mantenimiento de Usuario)
    {
        public string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required bool IsActive { get; set; } = true;

        // Extended properties
        public string? Role { get; set; } = "No Role Associated"; // Para mostrar en UI
    }
}
