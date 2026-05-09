using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.VoteScreen
{
    public class VoterOCRViewModel
    {
        [Required(ErrorMessage = "The Document Photo must be uploaded.")]
        [DataType(DataType.Upload)]
        public IFormFile DocumentPhoto { get; set; } // Document OCR
        public string DocumentNumber { get; set; } // From VoterIdentityViewModel
    }
}
