using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.VoteScreen
{
    public class VoterIdentityViewModel
    {
        [Required(ErrorMessage = "The Document Number field is required.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Document Number must be 11 digits")]
        public string DocumentNumber { get; set; }
    }
}
