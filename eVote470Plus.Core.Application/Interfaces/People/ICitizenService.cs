using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.Base;

namespace eVote470Plus.Core.Application.Interfaces.People
{
    public interface ICitizenService : IGenericService<CitizenDto>
    {
        Task<bool> ExistsDocumentNumberAsync(string documentNumber, int? excludeId = null);
        Task ToggleActiveAsync(int citizenId);
        Task<CitizenDto?> GetByDocumentNumberAsync(string documentNumber);
    }
}
