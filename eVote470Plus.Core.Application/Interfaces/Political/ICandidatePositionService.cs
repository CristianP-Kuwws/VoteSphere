using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.Political
{
    public interface ICandidatePositionService : IGenericService<CandidatePositionDto>
    {
        Task<bool> CandidateHasPositionInPartyAsync(int candidateId, int partyId);
        Task<CandidatePositionDto?> GetCandidatePositionInOriginPartyAsync(int candidateId);
        Task<List<CandidatePositionDto>> GetByPartyAsync(int partyId);
        Task<List<CandidatePositionDto>> GetAssignedCandidatesInPartyAsync(int partyId);
        Task<List<CandidatePositionDto>> GetByPositionAsync(int positionId);

    }
}
