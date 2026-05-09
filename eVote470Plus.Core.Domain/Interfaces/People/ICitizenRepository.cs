using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Domain.Interfaces.People
{
    public interface ICitizenRepository : IGenericRepository<Citizen>
    {
        Task<bool> ChangeActiveStatusAsync(int citizenId, bool isActive);
        Task<bool> HasVotedInElectionAsync(int citizenId, int electionId);
        Task<Citizen?> GetByDocumentNumberAsync(string documentNumber);
    }
}
