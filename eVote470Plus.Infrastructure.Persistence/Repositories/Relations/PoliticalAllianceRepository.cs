using eVote470Plus.Core.Domain.Common.Enum;
using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Relations;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Relations
{
    public class PoliticalAllianceRepository : GenericRepository<PoliticalAlliance>, IPoliticalAllianceRepository
    { 
        public PoliticalAllianceRepository(EVote470PlusContext context) : base(context)
        {
        }

        public async Task<List<PoliticalAlliance>> GetActiveAlliancesAsync(int partyId)
        {
            return await _context.PoliticalAlliances
                .Where(pa => (pa.SenderPartyId == partyId || pa.ReceiverPartyId == partyId)
                             && pa.Status == AllianceStatus.Accepted)
                .Include(pa => pa.SenderParty)
                .Include(pa => pa.ReceiverParty)
                .ToListAsync();
        }

        public async Task<List<PoliticalAlliance>> GetPendingReceivedRequestsAsync(int partyId)
        {
            return await _context.PoliticalAlliances
                .Where(pa => pa.ReceiverPartyId == partyId
                             && pa.Status == AllianceStatus.Pending)
                .Include(pa => pa.SenderParty)
                .ToListAsync();
        }

        public async Task<List<PoliticalAlliance>> GetSentRequestsAsync(int partyId)
        {
            return await _context.PoliticalAlliances
                .Where(pa => pa.SenderPartyId == partyId)
                .Include(pa => pa.ReceiverParty)
                .ToListAsync();
        }

        public async Task<bool> HasPendingRequestBetweenPartiesAsync(int party1Id, int party2Id)
        {
            return await _context.PoliticalAlliances
                .AnyAsync(pa => 
                    ((pa.SenderPartyId == party1Id && pa.ReceiverPartyId == party2Id) ||
                     (pa.SenderPartyId == party2Id && pa.ReceiverPartyId == party1Id)) &&
                    pa.Status == AllianceStatus.Pending);
        }
    }
}
