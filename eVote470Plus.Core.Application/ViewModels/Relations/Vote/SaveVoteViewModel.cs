using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Relations.Vote
{
    public class SaveVoteViewModel
    {
        public int VoteId { get; set; }
        public int CitizenId { get; set; }

        [Required(ErrorMessage = "At least one Candidate must be selected for vote.")]
        public int CandidateId { get; set; }
        public int ElectionId { get; set; }
        public int PoliticalPositionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
