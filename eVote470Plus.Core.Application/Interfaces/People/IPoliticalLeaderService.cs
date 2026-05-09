using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.People
{
    public interface IPoliticalLeaderService : IGenericService<PoliticalLeaderDto>
    {
        Task<PoliticalLeaderDto?> GetByIdentityUserIdAsync(string userId);
        Task<PoliticalLeaderDto?> GetByStringIdAsync(string id);
        Task<bool> DeleteByStringIdAsync(string id);
    }
}
