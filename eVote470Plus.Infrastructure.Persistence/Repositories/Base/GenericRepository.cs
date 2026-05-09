using eVote470Plus.Core.Domain.Interfaces.Base;
using eVote470Plus.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote470Plus.Infrastructure.Persistence.Repositories.Base
{
    public class GenericRepository<Entity> : IGenericRepository<Entity> where Entity : class
    {

        protected readonly EVote470PlusContext _context;
        public GenericRepository(EVote470PlusContext context)
        {
            _context = context;
        }

        public virtual async Task<Entity?> AddAsync(Entity entity)
        {
            try
            {
                await _context.Set<Entity>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to add the entity: {typeof(Entity).Name}.", ex);
            }
        }

        public virtual async Task<Entity?> UpdateAsync(int id, Entity entity)
        {
            try
            {
                var entry = await _context.Set<Entity>().FindAsync(id);

                if (entry != null)
                {
                    _context.Entry(entry).CurrentValues.SetValues(entity);
                    await _context.SaveChangesAsync();
                    return entry;
                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to update the entity: {typeof(Entity).Name} with id: {id}.", ex);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                var entry = await _context.Set<Entity>().FindAsync(id);

                if (entry != null)
                {
                    _context.Set<Entity>().Remove(entry);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to delete the entity: {typeof(Entity).Name} with id: {id}.", ex);
            }
        }

        public virtual async Task<List<Entity>> GetAllListAsync()
        {
            try
            {
                return await _context.Set<Entity>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to get all entities of type: {typeof(Entity).Name}.", ex);
            }
        }

        public virtual async Task<Entity?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Entity>().FindAsync(id); 
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to get the entity: {typeof(Entity).Name} with id: {id}.", ex);
            }
        }

        // Additional

        public virtual IQueryable<Entity> GetAllQuery()
        {
            try
            {
                return _context.Set<Entity>().AsQueryable();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to get query for entities of type: {typeof(Entity).Name}.", ex);
            }
        }

        public virtual IQueryable<Entity> GetAllQueryWithInclude(List<string> properties)
        {
            try
            {
                var query = _context.Set<Entity>().AsQueryable();

                foreach (var property in properties)
                {
                    query = query.Include(property);
                }

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to get query with includes for entities of type: {typeof(Entity).Name}.", ex);
            }
        }

        public virtual async Task<List<Entity?>> AddRangeAsync(List<Entity> entities)
        {
            try
            {
                await _context.Set<Entity>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to add range of entities of type: {typeof(Entity).Name}.", ex);
            }
        } 

        public virtual Task UpdateRange(List<Entity> entities)
        {
            try
            {
                _context.Set<Entity>().UpdateRange(entities);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to update range of entities of type: {typeof(Entity).Name}.", ex);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured while trying to save changes to the context for entities of type: {typeof(Entity).Name}.", ex);
            }
        }
    }
}
