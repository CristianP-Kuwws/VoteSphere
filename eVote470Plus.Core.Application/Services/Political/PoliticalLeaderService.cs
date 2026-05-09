using AutoMapper;
using eVote470Plus.Core.Application.Dtos.People;
using eVote470Plus.Core.Application.Interfaces.People;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.People;
using eVote470Plus.Core.Domain.Interfaces.People;

namespace eVote470Plus.Core.Application.Services.Political
{
    public class PoliticalLeaderService : GenericService<PoliticalLeader, PoliticalLeaderDto>, IPoliticalLeaderService
    {
        private readonly IPoliticalLeaderRepository _repository;
        private readonly IMapper _mapper;

        public PoliticalLeaderService(IPoliticalLeaderRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PoliticalLeaderDto?> GetByIdentityUserIdAsync(string userId)
        {
            try
            {
                var leader = await _repository.GetByIdentityUserIdAsync(userId);
                if (leader == null) return null;
                return _mapper.Map<PoliticalLeaderDto>(leader);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the political leader by user ID.", ex);
            }
        }

        // Override GetByIdAsync to work with string key
        public async Task<PoliticalLeaderDto?> GetByStringIdAsync(string id)
        {
            try
            {
                var entity = await _repository.GetByStringIdAsync(id);
                if (entity == null) return null;
                return _mapper.Map<PoliticalLeaderDto>(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the political leader by string ID.", ex);
            }
        }

        // Override DeleteAsync to work with string key
        public async Task<bool> DeleteByStringIdAsync(string id)
        {
            try
            {
                await _repository.DeleteByStringIdAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the political leader.", ex);
            }
        }
    }
}
