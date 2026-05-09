using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Political.PoliticalParty
{
    public class SavePoliticalPartyViewModel
    {
        public int PoliticalPartyId { get; set; }

        [Required(ErrorMessage = "The Name Field is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; } = "No description provided.";

        [Required(ErrorMessage = "The Acronym Field is required.")]
        [StringLength(10)]
        public required string Acronym { get; set; }
        public required bool IsActive { get; set; } = true;
        public IFormFile? LogoImg { get; set; }

        // Needed to preserve/pass existing logo URL on edit
        public string? LogoUrl { get; set; }
    }
}
