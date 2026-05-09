using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.Political
{
    public interface IPoliticalPartyService : IGenericService<PoliticalPartyDto>
    {
        Task ToggleActiveAsync(int id);
        Task<List<PoliticalPartyDto>> GetActivePartiesWithCandidatesAsync();

        Task<bool> ExistAcronymAsync(string acronym, int? excludeId = null);
    }
}
