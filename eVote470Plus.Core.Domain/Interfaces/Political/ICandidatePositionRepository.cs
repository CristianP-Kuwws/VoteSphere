using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Political
{
    public interface ICandidatePositionRepository : IGenericRepository<CandidatePosition>
    {
        Task<bool> CandidateHasPositionInPartyAsync(int candidateId, int partyId);
        Task<CandidatePosition?> GetCandidatePositionInOriginPartyAsync(int candidateId);
        Task<List<CandidatePosition>> GetByPartyAsync(int partyId);
        Task<List<CandidatePosition>> GetAssignedCandidateIdsInPartyAsync(int partyId);
        Task<List<CandidatePosition>> GetByPositionAsync(int positionId);

    }
}
