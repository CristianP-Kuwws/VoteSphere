using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.People
{
    public interface ICandidateService : IGenericService<CandidateDto> 
    {
        Task ToggleActiveAsync(int candidateId);
        Task<List<CandidateDto>> GetByPositionAsync(int positionId);
        Task<List<CandidateDto>> GetByPoliticalPartyAsync(int partyId);
        Task<bool> ExistsActiveCandidateForPartyAndPositionAsync(int partyId, int positionId);
    }
}
