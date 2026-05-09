using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.Political
{
    public interface IPoliticalPositionService : IGenericService<PoliticalPositionDto>
    {
        Task ToggleActiveAsync(int id);
        Task<List<PoliticalPositionDto>> GetActivePositionsAsync();

    }
}
