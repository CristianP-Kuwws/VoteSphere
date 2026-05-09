using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "The Name is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The LastName is required.")]
        [StringLength(60)]
        public required string LastName { get; set; }
        public required bool IsActive { get; set; } 

    }
}
