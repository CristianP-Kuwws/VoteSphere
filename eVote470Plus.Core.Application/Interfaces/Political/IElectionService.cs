using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Dtos.Results;
using eVote470Plus.Core.Application.Interfaces.Base;
using System.ComponentModel;

namespace eVote470Plus.Core.Application.Interfaces.Political
{
    public interface IElectionService : IGenericService<ElectionDto>
    {
        Task<bool> ChangeActiveStatusAsync(int electionId);
        Task<ElectionDto?> GetActiveElectionAsync();
        Task<bool> HasActiveElectionAsync();
        Task<List<ElectionDto>> GetElectionsByYearAsync(int year);
        Task FinalizeElectionAsync(int  electionId);
        Task<List<int>> GetElectionYearsAsync();
        Task<List<ElectionResultDto>> GetElectionResultsAsync(int electionId);
        Task<ElectionValidationResult> CanCreateElection();
    }
}
