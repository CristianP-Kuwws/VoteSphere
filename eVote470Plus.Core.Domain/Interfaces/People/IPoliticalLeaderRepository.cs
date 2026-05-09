using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.People
{
    public interface IPoliticalLeaderRepository : IGenericRepository<PoliticalLeader>
    {
        Task<PoliticalLeader?> GetByIdentityUserIdAsync(string userId);

        Task<PoliticalLeader?> GetByStringIdAsync(string id);

        Task DeleteByStringIdAsync(string id);

    }
}
