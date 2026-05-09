namespace eVote470Plus.Core.Application.Interfaces.Base
{
    public interface IGenericService<DtoModel> 
        where DtoModel : class
    {
        Task<DtoModel?> AddAsync(DtoModel dtoModel);
        Task<DtoModel?> UpdateAsync(DtoModel dtoModel, int id);
        Task<bool> DeleteAsync(int id);
        Task<List<DtoModel>> GetAllAsync();
        Task<DtoModel?> GetByIdAsync(int id);
    }
}