using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Political
{
    public interface IPoliticalPositionRepository : IGenericRepository<PoliticalPosition>
    {
        Task<bool> ChangeActiveStatusAsync(int positionId, bool isActive);
        Task<List<PoliticalPosition>> GetActivePositionsAsync();
    }
}
