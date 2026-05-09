using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.People.PoliticalLeader
{
    public class SavePoliticalLeaderAssignmentViewModel
    {
        [Required(ErrorMessage = "The Political Leader field is required.")]
        public string IdentityUserId { get; set; }

        [Required(ErrorMessage = "The Political Party field is required.")]
        public int PoliticalPartyId { get; set; }
    }
}
