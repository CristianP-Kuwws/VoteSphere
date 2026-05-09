using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Relations
{
    public interface IPoliticalAllianceRepository : IGenericRepository<PoliticalAlliance>
    {
        Task<List<PoliticalAlliance>> GetPendingReceivedRequestsAsync(int partyId);
        Task<List<PoliticalAlliance>> GetSentRequestsAsync(int partyId);
        Task<List<PoliticalAlliance>> GetActiveAlliancesAsync(int partyId);
        Task<bool> HasPendingRequestBetweenPartiesAsync(int party1Id, int party2Id);
    }
}
