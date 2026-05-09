using eVote470Plus.Core.Domain.Entities.Relations;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.Relations
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<int> GetVoteCountByCandidateAsync(int candidateId, int electionId);
        Task<int> GetTotalVotesByElectionAsync(int electionId);
        Task<List<Vote>> GetVotesByPositionAndElectionAsync(int positionId, int electionId);
        Task<bool> HasVotedInElectionAsync(int citizenId, int electionId);
        Task<Vote?> GetVoteByCitizenPositionAsync(int citizenId, int electonId, int positionId);
        Task<List<int>> GetVotedPositionIdsAsync(int citizenId, int electionId);
        Task<List<Vote>> GetVotesWithDetails(int electionId);
        Task<bool> HasCompletedVotingAsync(int citizenId, int electionId);

    }
}
