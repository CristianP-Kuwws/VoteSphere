using AutoMapper;
using eVote470Plus.Core.Application.Interfaces.Base;
using eVote470Plus.Core.Domain.Interfaces.Base;

namespace eVote470Plus.Core.Application.Services.Base
{
    public class GenericService<Entity, DtoModel> : IGenericService<DtoModel>
        where Entity : class
        where DtoModel : class
    {
        private readonly IGenericRepository<Entity> _repository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<Entity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public virtual async Task<DtoModel?> AddAsync(DtoModel dtoModel)
        {
            try
            {
                Entity entity = _mapper.Map<Entity>(dtoModel);
                Entity? returnEntity = await _repository.AddAsync(entity);

                if (returnEntity == null)
                    return null;

                return _mapper.Map<DtoModel>(returnEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddAsync: {ex.Message}"); 
                return null;
            }


        }

        public virtual async Task<DtoModel?> UpdateAsync(DtoModel dtoModel, int id)
        {
            try
            {
                Entity entity = _mapper.Map<Entity>(dtoModel);
                Entity? returnEntity = await _repository.UpdateAsync(id, entity);

                if (returnEntity == null)
                    return null;

                return _mapper.Map<DtoModel>(returnEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateAsync: {ex.Message}");
                return null;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteAsync: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<List<DtoModel>> GetAllAsync()
        {
            try
            {
                var listEntities = await _repository.GetAllListAsync() ?? new List<Entity>();
                var listDto = _mapper.Map<List<DtoModel>>(listEntities);

                return listDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllAsync: {ex.Message}");
                return new List<DtoModel>();
            }
        }

        public virtual async Task<DtoModel?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);

                if (entity == null)
                    return null;

                DtoModel dto = _mapper.Map<DtoModel>(entity);
                return dto;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        
    }
}
