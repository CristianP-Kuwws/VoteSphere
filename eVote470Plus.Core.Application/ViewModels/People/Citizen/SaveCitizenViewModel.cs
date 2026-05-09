using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.People.Citizen
{
    public class SaveCitizenViewModel
    {
        public int CitizenId { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(60)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "The Document Number Field is required")]
        [StringLength(11)]
        public required string DocumentNumber { get; set; }

        [Required(ErrorMessage = "The Email Field is required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [StringLength(100)]
        public required string Email { get; set; }
        public required bool IsActive { get; set; } = true;
    }
}
