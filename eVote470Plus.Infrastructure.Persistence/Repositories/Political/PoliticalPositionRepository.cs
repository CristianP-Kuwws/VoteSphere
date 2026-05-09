using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Political
{
    public class PoliticalPositionRepository : GenericRepository<PoliticalPosition>, IPoliticalPositionRepository
    {
        public PoliticalPositionRepository(EVote470PlusContext context) : base(context)
        {

        }

        public async Task<bool> ChangeActiveStatusAsync(int positionId, bool isActive)
        {
            var position = await GetByIdAsync(positionId);
            if (position == null) return false;

            position.IsActive = isActive; 

            if (!isActive)
            {
                var candidatePositions = await _context.CandidatePositions
                    .Where(cp => cp.PoliticalPositionId == positionId)
                    .Include(cp => cp.Candidate)
                    .ToListAsync();

                foreach (var cp in candidatePositions)
                {
                    cp.Candidate.IsActive = false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PoliticalPosition>> GetActivePositionsAsync()
        {
            return await _context.PoliticalPositions
                .Where(p => p.IsActive == true)
                .ToListAsync();
        }
    }
}
