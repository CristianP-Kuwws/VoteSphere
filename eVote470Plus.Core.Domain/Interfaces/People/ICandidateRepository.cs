using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.People
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<bool> ToggleActiveAsync(int candidateId, bool isActive);
        Task<List<Candidate>> GetByPositionAsync(int positionId);
        Task<List<Candidate>> GetByPoliticalPartyAsync(int partyId);
        Task<bool> ExistsActiveCandidateForPartyAndPositionAsync(int partyId, int positionId);
    }
}
