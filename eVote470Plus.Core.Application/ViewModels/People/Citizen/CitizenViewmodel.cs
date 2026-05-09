namespace eVote470Plus.Core.Application.ViewModels.People.Citizen
{
    public class CitizenViewmodel
    {
        public int CitizenId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string DocumentNumber { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
    }
}
