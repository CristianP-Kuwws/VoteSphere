using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Political
{
    public class PoliticalPartyRepository : GenericRepository<PoliticalParty>, IPoliticalPartyRepository
    {
        public PoliticalPartyRepository(EVote470PlusContext context) : base(context)
        {

        }

        public async Task<bool> ChangeActiveStatusAsync(int partyId, bool isActive)
        {
            var party = await GetByIdAsync(partyId);
            if (party == null) return false;

            party.IsActive = isActive;

            if (!isActive)
            {
                var candidates = await _context.Candidates
                    .Where(c => c.PoliticalPartyId == partyId)
                    .ToListAsync();

                foreach (var candidate in candidates)
                {
                    candidate.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PoliticalParty>> GetActivePartiesAsync()

        {
            return await _context.PoliticalParties
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<List<PoliticalParty>> GetActivePartiesWithCandidatesAsync()
        {
            return await _context.PoliticalParties
                .Where(p => p.IsActive)
                .Include(p => p.Candidates.Where(c => c.IsActive))
                .ToListAsync();
        }

        public async Task<PoliticalParty?> GetByAcronymAsync(string acronym)
        {
           return await _context.PoliticalParties
                .FirstOrDefaultAsync(p => p.Acronym == acronym);
        }

    }
}
