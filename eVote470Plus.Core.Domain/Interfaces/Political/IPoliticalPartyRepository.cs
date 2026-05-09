using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Political
{
    public interface IPoliticalPartyRepository : IGenericRepository<PoliticalParty>
    {
        Task<bool> ChangeActiveStatusAsync(int partyId, bool isActive);
        Task<List<PoliticalParty>> GetActivePartiesWithCandidatesAsync();

        Task<PoliticalParty?> GetByAcronymAsync(string acronym);

        Task<List<PoliticalParty>> GetActivePartiesAsync();
    }
}
