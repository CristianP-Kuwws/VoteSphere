using System.ComponentModel.DataAnnotations;

namespace eVote470Plus.Core.Application.ViewModels.Political.PoliticalPosition
{
    public class SavePoliticalPositionViewModel
    {
        public int PoliticalPositionId { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(60)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        [StringLength(250)]
        public required string Description { get; set; }
        public required bool IsActive { get; set; } = true;
    }
}
