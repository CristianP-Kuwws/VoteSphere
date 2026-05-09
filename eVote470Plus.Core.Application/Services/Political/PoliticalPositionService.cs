using AutoMapper;
using eVote470Plus.Core.Application.Dtos.Political;
using eVote470Plus.Core.Application.Interfaces.Political;
using eVote470Plus.Core.Application.Services.Base;
using eVote470Plus.Core.Domain.Entities.Political;
using eVote470Plus.Core.Domain.Interfaces.Political;

namespace eVote470Plus.Core.Application.Services.Political
{
    public class PoliticalPositionService : GenericService<PoliticalPosition, PoliticalPositionDto>, IPoliticalPositionService
    {
        private readonly IPoliticalPositionRepository _repository;
        private readonly IMapper _mapper;

        public PoliticalPositionService(IPoliticalPositionRepository repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PoliticalPositionDto>> GetActivePositionsAsync()
        {
            try
            {
                var activePositions = await _repository.GetActivePositionsAsync();
                return _mapper.Map<List<PoliticalPositionDto>>(activePositions);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving active political positions.", ex);
            }
        }

        public async Task ToggleActiveAsync(int id)
        {
            try
            {
                var position = await _repository.GetByIdAsync(id);

                if (position == null)
                {
                    throw new KeyNotFoundException($"Political Position not found.");
                }

                bool newStatus = !position.IsActive;
                bool success = await _repository.ChangeActiveStatusAsync(id, newStatus);

                if (!success)
                {
                    throw new Exception("Failed to change active status.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while toggling the active status of the Political Position.", ex);
            }
        }

    }
}
