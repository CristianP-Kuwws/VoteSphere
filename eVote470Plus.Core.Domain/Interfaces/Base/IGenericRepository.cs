namespace eVote470Plus.Core.Domain.Interfaces.Base
{
    public interface IGenericRepository<Entity> where Entity : class
    {
        Task<Entity?> AddAsync(Entity entity);
        Task<Entity?> UpdateAsync(int id, Entity entity);
        Task DeleteAsync(int id);
        Task<List<Entity>> GetAllListAsync();
        Task<Entity?> GetByIdAsync(int id);

        // Additional
        IQueryable<Entity> GetAllQuery();
        IQueryable<Entity> GetAllQueryWithInclude(List<string> properties);
        Task<List<Entity?>> AddRangeAsync(List<Entity> entities);
        Task UpdateRange(List<Entity> entities);
        Task SaveAsync();
    }
}
