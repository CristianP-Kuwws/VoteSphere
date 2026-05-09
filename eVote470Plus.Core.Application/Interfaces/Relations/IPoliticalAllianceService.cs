using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.Relations
{
    public interface IPoliticalAllianceService : IGenericService<PoliticalAllianceDto>
    {
        Task<List<PoliticalAllianceDto>> GetPendingReceivedRequestsAsync(int partyId);
        Task<List<PoliticalAllianceDto>> GetSentRequestsAsync(int partyId);
        Task<List<PoliticalAllianceDto>> GetActiveAlliancesAsync(int partyId);
        Task<bool> HasPendingRequestBetweenPartiesAsync(int party1Id, int party2Id);
        Task AcceptAllianceAsync(int allianceId);
        Task RejectAllianceAsync(int allianceId);
        Task EndAllianceAsync(int allianceId, int currentPartyId);

    }
}
