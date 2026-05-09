using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Political
{
    public class CandidatePositionRepository : GenericRepository<CandidatePosition>, ICandidatePositionRepository
    {
        public CandidatePositionRepository(EVote470PlusContext context) : base(context)
        {
        }

        public async Task<bool> CandidateHasPositionInPartyAsync(int candidateId, int partyId)
        {
            return await _context.CandidatePositions
                .AnyAsync(cp => cp.CandidateId == candidateId
                            && cp.PoliticalPartyId == partyId);
        }

        public async Task<CandidatePosition?> GetCandidatePositionInOriginPartyAsync(int candidateId)
        {
            var candidate = await _context.Candidates
                .Include(c => c.CandidatePositions)
                .FirstOrDefaultAsync(c => c.CandidateId == candidateId);

            return candidate?.CandidatePositions
                .FirstOrDefault(cp => cp.PoliticalPartyId == candidate.PoliticalPartyId);
        }

        public async Task<List<CandidatePosition>> GetByPartyAsync(int partyId)
        {
            return await _context.CandidatePositions
                .Where(cp => cp.PoliticalPartyId == partyId)
                .Include(cp => cp.Candidate)
                .Include(cp => cp.PoliticalPosition)
                .ToListAsync();
        }

        public async Task<List<CandidatePosition>> GetAssignedCandidateIdsInPartyAsync(int partyId)
        {
            return await _context.CandidatePositions
                .Where(cp => cp.PoliticalPartyId == partyId)
                .ToListAsync();
        }

        public async Task<List<CandidatePosition>> GetByPositionAsync(int positionId)
        {
            return await _context.CandidatePositions
                .Where(cp => cp.PoliticalPositionId == positionId)
                .Include(cp => cp.Candidate)
                .ToListAsync();
        }
    }
}
