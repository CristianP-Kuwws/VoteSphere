using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Political.Election
{
    public class SaveElectionViewModel
    {
        public int ElectionId { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [MaxLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The ElectionDate field is required.")]
        public DateTime ElectionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
