using eVote470Plus.Core.Domain.Entities.Relations;

namespace eVote470Plus.Core.Domain.Entities.People
{
    public class Citizen 
    {
        public int CitizenId { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string DocumentNumber { get; set; }
        public required string Email { get; set; } 
        public required bool IsActive { get; set; } 

        // Navigation properties
        public ICollection<Vote>? Votes { get; set; }
    }
}
