using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Political
{
    public interface IElectionRepository : IGenericRepository<Election>
    {
        Task<bool> ChangeActiveStatusAsync(int electionId, bool isActive);

        Task<Election?> GetActiveElectionAsync();
        Task<bool> HasActiveElectionAsync();
        Task<List<Election>> GetElectionsByYearAsync(int year);

        Task<List<int>> GetElectionYearAsync();

    }
}
