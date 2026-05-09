using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.People.Candidate
{
    public class SaveCandidateViewModel
    {
        public int CandidateId { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(60)]
        public required string LastName { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? CandidatePhoto { get; set; }

        // Needed to preserve/pass existing photo URL on edit
        public string? PhotoUrl { get; set; }

        // public required bool IsActive { get; set; } = true;
        //public int? PoliticalPartyId { get; set; }

    }
}
