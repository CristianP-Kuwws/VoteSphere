using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Political
{
    public class ElectionRepository : GenericRepository<Election>, IElectionRepository
    {
        public ElectionRepository(EVote470PlusContext context) : base(context)
        {
        }

        public async Task<bool> ChangeActiveStatusAsync(int electionId, bool isActive)
        {
            try
            {
                var election = await GetByIdAsync(electionId);

                if (election == null)
                {
                    return false;
                }

                election.IsActive = isActive;
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to activate Election: {nameof(Election)}", ex);
            }
        }

        public async Task<Election?> GetActiveElectionAsync()
        {
            return await _context.Elections
                .Include(e => e.ElectionPositions)
                    .ThenInclude(ep => ep.PoliticalPosition)
                .FirstOrDefaultAsync(e => e.IsActive);
        }

        public async Task<List<Election>> GetElectionsByYearAsync(int year)
        {
            return await _context.Elections
                .Where(e => e.ElectionDate.Year == year)
                .OrderByDescending(e => e.ElectionDate)
                .ToListAsync();
        }

        public async Task<List<int>> GetElectionYearAsync()
        {
            return await _context.Elections
                .Select(e => e.ElectionDate.Year)
                .Distinct()
                .OrderBy(year => year)
                .ToListAsync();
        }

        public async Task<bool> HasActiveElectionAsync()
        {
            return await _context.Elections
                .AnyAsync(e => e.IsActive);
        }
    }
}
