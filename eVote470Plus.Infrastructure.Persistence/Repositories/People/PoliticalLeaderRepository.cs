using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using eVote470Plus.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.People
{
    public class PoliticalLeaderRepository : GenericRepository<PoliticalLeader>, IPoliticalLeaderRepository
    {
        public PoliticalLeaderRepository(EVote470PlusContext context) : base(context)
        {

        }

        public async Task<PoliticalLeader?> GetByIdentityUserIdAsync(string userId)
        {
            try
            {
                return await _context.Set<PoliticalLeader>()
                    .Include(pl => pl.PoliticalParty)
                    .FirstOrDefaultAsync(pl => pl.IdentityUserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the political leader by identity user ID.", ex);
            }
        }

        public async Task<PoliticalLeader?> GetByStringIdAsync(string id)
        {
            try
            {
                return await _context.PoliticalLeaders
                    .FirstOrDefaultAsync(pl => pl.PoliticalLeaderId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the political leader.", ex);
            }
        }

        public async Task DeleteByStringIdAsync(string id)
        {
            try
            {
                var entity = await _context.PoliticalLeaders
                    .FirstOrDefaultAsync(pl => pl.PoliticalLeaderId == id);
                if (entity != null)
                {
                    _context.PoliticalLeaders.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the political leader.", ex);
            }
        }
    }

}
