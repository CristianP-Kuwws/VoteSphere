using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.People.ApplicationUser
{
    public class EditApplicationUserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(60)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "The Email Field is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "The UserName Field is required")]
        public required string UserName { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be 6 characters minimum.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Must contain Capital and Small Letters, as well as numbers.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string? ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = true;

        public required string Role { get; set; } = "No Role Associated";
    }
}
