using eVote470Plus.Core.Application.Dtos.Relations;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.Relations
{
    public interface IVoteService : IGenericService<VoteDto>
    {
        Task<int> GetVoteCountByCandidateAsync(int candidateId, int electionId);
        Task<int> GetTotalVotesByElectionAsync(int electionId);
        Task<List<VoteDto>> GetVotesByPositionAndElectionAsync(int positionId, int electionId);
        Task CastVoteAsync(int citizenId, int electionId, int candidateId, int positionId);
        Task<List<int>> GetVotedPositionIdsAsync(int citizenId, int electionId);
        Task<bool> HasVotedInElectionAsync(int citizenId, int electionId);
        Task<bool> HasCompletedVotingAsync(int citizenId, int electionId);

    }
}
