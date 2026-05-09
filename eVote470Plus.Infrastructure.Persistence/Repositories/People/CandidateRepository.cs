using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.People
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(EVote470PlusContext context) : base(context)
        {
            
        }

        public async Task<bool> ToggleActiveAsync(int candidateId, bool isActive)
        {
            try
            {
                var candidate = await GetByIdAsync(candidateId);

                if (candidate == null)
                {
                    return false;
                }

                candidate.IsActive = isActive;
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to activate Candidate: {nameof(Candidate)}", ex);
            }
        }

        public async Task<List<Candidate>> GetByPositionAsync(int positionId)
        {
            return await _context.Candidates
                .Where(c => c.CandidatePositions.Any(cp => cp.PoliticalPositionId == positionId)
                    && c.IsActive)
                .Include(c => c.PoliticalParty)
                .ToListAsync();
        }

        public async Task<List<Candidate>> GetByPoliticalPartyAsync(int partyId)
        {
            return await _context.Candidates
                .Where(c => c.PoliticalPartyId == partyId)
                .Include(c => c.CandidatePositions)
                    .ThenInclude(cp => cp.PoliticalPosition)
                .ToListAsync();
        }

        public async Task<bool> ExistsActiveCandidateForPartyAndPositionAsync(int partyId, int positionId)
        {
            return await _context.CandidatePositions
                .AnyAsync(cp => cp.PoliticalPartyId == partyId
                            && cp.PoliticalPositionId == positionId
                            && cp.Candidate.IsActive);
        }

    }
}
