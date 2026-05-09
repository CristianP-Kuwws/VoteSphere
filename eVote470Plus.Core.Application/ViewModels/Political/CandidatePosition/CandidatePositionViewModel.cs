namespace eVote470Plus.Core.Application.ViewModels.Political.CandidatePosition
{
    public class CandidatePositionViewModel
    {
        public int CandidatePositionId { get; set; }
        public int CandidateId { get; set; }
        public int PoliticalPositionId { get; set; }
        public int PoliticalPartyId { get; set; }

        // Extended properties
        public string CandidateName { get; set; } // Nombre + Apellido
        public string PoliticalPositionName { get; set; }

        // 
        public bool CandidateIsActive { get; set; }
        public bool PositionIsActive { get; set; }
    }
}
